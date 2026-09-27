using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography;
using Genbox.HttpBuilders.Enums;
using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class PutObjectTests : TestBase
{
    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, ObjectCannedAcl.AuthenticatedRead, ObjectCannedAcl.AwsExecRead, ObjectCannedAcl.BucketOwnerFullControl, ObjectCannedAcl.BucketOwnerRead, ObjectCannedAcl.Private, ObjectCannedAcl.PublicRead, ObjectCannedAcl.PublicReadWrite)]
    [MultipleProvidersWithData(S3Provider.BackBlazeB2, ObjectCannedAcl.Private)]
    [MultipleProvidersWithData(S3Provider.GoogleCloudStorage, ObjectCannedAcl.AuthenticatedRead, ObjectCannedAcl.BucketOwnerFullControl, ObjectCannedAcl.BucketOwnerRead, ObjectCannedAcl.Private, ObjectCannedAcl.PublicRead)]
    public async Task PutObjectCannedAcl(S3Provider _, string bucket, ISimpleClient client, ObjectCannedAcl acl)
    {
        string key = $"{nameof(PutObjectCannedAcl)}-{acl}-{Guid.NewGuid():N}";
        await TemporaryObject.WithAsync(client, bucket, key,
            () => client.PutObjectAsync(bucket, key, null, r => r.Acl = acl, TestContext.Current.CancellationToken),
            async _ =>
            {
                GetObjectAclResponse read = await client.GetObjectAclAsync(bucket, key, token: TestContext.Current.CancellationToken);
                Assert.Equal(200, read.StatusCode);
                S3Grant owner = Assert.Single(read.Grants, grant => grant.Permission == S3Permission.FullControl);
                AssertOwnerGrant(read.Owner, owner);
                List<S3Grant> additional = read.Grants.Where(grant => !ReferenceEquals(grant, owner)).ToList();

                if (acl is ObjectCannedAcl.Private or ObjectCannedAcl.BucketOwnerRead or ObjectCannedAcl.BucketOwnerFullControl)
                {
                    // The uploader owns the bucket too, so owner-read/full-control collapse to its existing owner grant.
                    Assert.Empty(additional);
                    return;
                }

                if (acl == ObjectCannedAcl.AwsExecRead)
                {
                    S3Grant ec2 = Assert.Single(additional);
                    Assert.Equal(S3Permission.Read, ec2.Permission);
                    Assert.Equal(GrantType.CanonicalUser, ec2.Grantee.Type);
                    Assert.Equal("6aa5a366c34c1cbe25dc49211496e913e0351eb0e8c37aa3477e40942ec6b97c", ec2.Grantee.Id);
                    return;
                }

                string group = acl == ObjectCannedAcl.AuthenticatedRead ? "AuthenticatedUsers" : "AllUsers";
                Assert.All(additional, grant =>
                {
                    Assert.Equal(GrantType.Group, grant.Grantee.Type);
                    Assert.Equal("http://acs.amazonaws.com/groups/global/" + group, grant.Grantee.Uri);
                });
                S3Permission[] permissions = acl == ObjectCannedAcl.PublicReadWrite ? [S3Permission.Read, S3Permission.Write] : [S3Permission.Read];
                Assert.Equal(permissions.Order(), additional.Select(grant => grant.Permission).Order());
            });
    }

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, LockMode.Compliance, LockMode.Governance)]
    public async Task PutObjectLockMode(S3Provider provider, string _, ISimpleClient client, LockMode lockMode) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        DateTimeOffset lockRetainUntil = DateTimeOffset.UtcNow.AddSeconds(10);

        //We add a unique guid to prevent contamination across runs
        string objectKey = $"{nameof(PutObjectLockMode)}-{lockMode}-{Guid.NewGuid()}";

        PutObjectResponse putResp = await client.PutObjectStringAsync(bucket, objectKey, "bla", null, r =>
        {
            r.LockMode = lockMode;
            r.LockRetainUntil = lockRetainUntil;
        }, TestContext.Current.CancellationToken);
        Assert.Equal(200, putResp.StatusCode);

        using GetObjectResponse getResp = await client.GetObjectAsync(bucket, objectKey, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, getResp.StatusCode);
        Assert.Equal(lockMode, getResp.LockMode);
        Assert.Equal(lockRetainUntil.UtcDateTime, getResp.LockRetainUntil!.Value.UtcDateTime, TimeSpan.FromSeconds(1));
    }, request => request.EnableObjectLocking = true);

    [Theory]
    [MultipleProvidersWithData(S3Provider.All, "NormalFile", "This/Should/Look/Like/Directories/File.txt", "_\\_", "~", "/", " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~/")]
    public async Task PutObjectValidCharacters(S3Provider provider, string _, ISimpleClient client, string name) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        PutObjectResponse putResp = await client.PutObjectStringAsync(bucket, name, "test", token: TestContext.Current.CancellationToken);
        Assert.Equal(200, putResp.StatusCode);

        using GetObjectResponse getResp = await client.GetObjectAsync(bucket, name, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, getResp.StatusCode);
        Assert.Equal("test", await getResp.Content.AsStringAsync(token: TestContext.Current.CancellationToken));
    });

    [Theory]
    [MultipleProvidersWithData(S3Provider.All & ~S3Provider.Wasabi, ".", "\0")]
    [MultipleProvidersWithData(S3Provider.Wasabi, ".")] //Wasabi seems to allow null bytes
    public async Task PutObjectInvalidCharacters(S3Provider provider, string _, ISimpleClient client, string name) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        //These 2 test cases came after an exhaustive search in the whole UTF-16 character space.
        PutObjectResponse putResp = await client.PutObjectAsync(bucket, name, null, token: TestContext.Current.CancellationToken);

        // These keys reach the service: dot-path normalization and NUL handling differ by provider.
        if (name == "." && provider == S3Provider.GoogleCloudStorage)
            Assert.Equal(302, putResp.StatusCode);
        else if (name == "\0" && provider != S3Provider.GoogleCloudStorage)
            Assert.Equal(400, putResp.StatusCode);
        else
        {
            Assert.Equal(403, putResp.StatusCode);
            Assert.Equal(ErrorCode.SignatureDoesNotMatch, putResp.Error?.Code);
        }
    });

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, SseAlgorithm.Aes256, SseAlgorithm.AwsKms)]
    [MultipleProvidersWithData(S3Provider.BackBlazeB2, SseAlgorithm.Aes256)]
    public async Task PutObjectServerSideEncryption(S3Provider provider, string _, ISimpleClient client, SseAlgorithm algorithm) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(PutObjectServerSideEncryption), null, r => r.SseAlgorithm = algorithm, TestContext.Current.CancellationToken);
        Assert.Equal(200, putResp.StatusCode);
        Assert.Equal(algorithm, putResp.SseAlgorithm);

        using GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(PutObjectServerSideEncryption), token: TestContext.Current.CancellationToken);
        Assert.Equal(200, getResp.StatusCode);
        Assert.Equal(algorithm, getResp.SseAlgorithm);
    });

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, StorageClass.Standard, StorageClass.DeepArchive, StorageClass.Glacier, StorageClass.IntelligentTiering, StorageClass.OneZoneIa, StorageClass.ReducedRedundancy, StorageClass.StandardIa)]
    [MultipleProvidersWithData(S3Provider.BackBlazeB2 | S3Provider.GoogleCloudStorage, StorageClass.Standard)]
    public async Task PutObjectStorageClass(S3Provider provider, string _, ISimpleClient client, StorageClass storageClass) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(PutObjectStorageClass) + "-" + storageClass, null, r => r.StorageClass = storageClass, TestContext.Current.CancellationToken);
        Assert.Equal(200, putResp.StatusCode);
        Assert.Equal(storageClass, putResp.StorageClass);

        if (storageClass is StorageClass.DeepArchive or StorageClass.Glacier)
            return;

        using GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(PutObjectStorageClass) + "-" + storageClass, token: TestContext.Current.CancellationToken);
        Assert.Equal(storageClass, getResp.StorageClass);
        Assert.Equal(200, getResp.StatusCode);
    });

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, S3Permission.Read, S3Permission.ReadAcp, S3Permission.WriteAcp, S3Permission.FullControl)]
    public async Task CustomPermission(S3Provider _, string bucket, ISimpleClient client, S3Permission permission) => await CheckCustomPermissionAsync(client, bucket, permission, true);

    [Theory]
    [MultipleProvidersWithData(S3Provider.GoogleCloudStorage, S3Permission.Read, S3Permission.ReadAcp, S3Permission.WriteAcp, S3Permission.FullControl)]
    public async Task UnsupportedCustomGrantHeadersAreIgnored(S3Provider _, string bucket, ISimpleClient client, S3Permission permission) => await CheckCustomPermissionAsync(client, bucket, permission, false);

    private static async Task CheckCustomPermissionAsync(ISimpleClient client, string bucket, S3Permission permission, bool supported)
    {
        const string group = "http://acs.amazonaws.com/groups/global/AuthenticatedUsers";
        string key = "custom-grant-" + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectAsync(bucket, key, null, r =>
        {
            switch (permission)
            {
                case S3Permission.Read: r.AclGrantRead.AddGroup(group); break;
                case S3Permission.ReadAcp: r.AclGrantReadAcp.AddGroup(group); break;
                case S3Permission.WriteAcp: r.AclGrantWriteAcp.AddGroup(group); break;
                case S3Permission.FullControl: r.AclGrantFullControl.AddGroup(group); break;
            }
        }, TestContext.Current.CancellationToken), async _ =>
        {
            GetObjectAclResponse read = await client.GetObjectAclAsync(bucket, key, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, read.StatusCode);

            if (supported)
            {
                S3Grant grant = Assert.Single(read.Grants, item => item.Grantee.Uri == group);
                Assert.Equal(GrantType.Group, grant.Grantee.Type);
                Assert.Equal(permission, grant.Permission);
            }
            else
            {
                // GCS accepts the S3 custom-grant headers but retains only the owner ACL.
                S3Grant ownerGrant = Assert.Single(read.Grants);
                AssertOwnerGrant(read.Owner, ownerGrant);
                Assert.DoesNotContain(read.Grants, grant => grant.Grantee.Uri == group);
            }
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task PutObjectCacheControl(S3Provider _, string bucket, ISimpleClient client)
    {
        string key = nameof(PutObjectCacheControl) + "-" + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectAsync(bucket, key, null, r => r.CacheControl.Add(CacheControlType.MaxAge, 100), TestContext.Current.CancellationToken), async _ =>
        {
            using GetObjectResponse response = await client.GetObjectAsync(bucket, key, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("max-age=100", response.CacheControl);
        });
    }

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3 | S3Provider.GoogleCloudStorage, "disposition", "encoding", "type")]
    public async Task PutObjectContentProperties(S3Provider provider, string bucket, ISimpleClient client, string property)
    {
        string key = nameof(PutObjectContentProperties) + "-" + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectStringAsync(bucket, key, "test", null, r =>
        {
            if (property == "disposition") r.ContentDisposition.Set(ContentDispositionType.Attachment, "filename.jpg");
            if (property == "encoding") r.ContentEncoding.Add(ContentEncodingType.Identity);
            if (property == "type") r.ContentType.Set(MediaType.Text_plain, Charset.Utf_8);
        }, TestContext.Current.CancellationToken), async _ =>
        {
            using GetObjectResponse response = await client.GetObjectAsync(bucket, key, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(4, response.ContentLength);
            if (property == "disposition") Assert.Equal("attachment; filename*=\"filename.jpg\"", response.ContentDisposition);

            // GCS omits the no-op identity encoding on read-back.
            if (property == "encoding") Assert.Equal(provider == S3Provider.GoogleCloudStorage ? null : "identity", response.ContentEncoding);
            if (property == "type") Assert.Equal("text/plain; charset=utf-8", response.ContentType);
        });
    }

    [Theory]
    [MultipleProvidersWithData(S3Provider.All & ~S3Provider.Wasabi, 2047)]
    [MultipleProvidersWithData(S3Provider.Wasabi, 2036)] //Wasabi seems to be off by 10
    public async Task PutObjectLargeMetadata(S3Provider _, string bucket, ISimpleClient client, int limit)
    {
        string value = new string('b', limit);
        string key = nameof(PutObjectLargeMetadata) + "-" + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectAsync(bucket, key, null, r =>
        {
            //Amazon ignores the metadata prefix and header separator, so we just need to count key length + value length
            r.Metadata.Add("a", value);
        }, TestContext.Current.CancellationToken), async _ =>
        {
            using GetObjectResponse response = await client.GetObjectAsync(bucket, key, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(value, response.Metadata!["a"]);
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)]
    public async Task PutObjectLegalHold(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        Assert.Equal(200, (await client.PutObjectStringAsync(bucket, nameof(PutObjectLegalHold), "test", null, r => r.LockLegalHold = true, TestContext.Current.CancellationToken)).StatusCode);

        using GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(PutObjectLegalHold), token: TestContext.Current.CancellationToken);
        Assert.Equal(200, getResp.StatusCode);
        Assert.True(getResp.LockLegalHold);

        HeadObjectResponse headResp = await client.HeadObjectAsync(bucket, nameof(PutObjectLegalHold), token: TestContext.Current.CancellationToken);
        Assert.Equal(200, headResp.StatusCode);
        Assert.True(headResp.LockLegalHold);
    }, request => request.EnableObjectLocking = true);

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task PutObjectLifecycle(S3Provider _, string bucket, ISimpleClient client)
    {
        string key = nameof(PutObjectLifecycle) + Guid.NewGuid().ToString("N");
        DateTime expected = DateTime.UtcNow.AddDays(2).Date;
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectAsync(bucket, key, null, token: TestContext.Current.CancellationToken), put =>
        {
            Assert.Equal(expected, put.LifeCycleExpiresOn!.Value.UtcDateTime.Date);
            Assert.Equal("ExpireAll", put.LifeCycleRuleId);
            return Task.CompletedTask;
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task PutObjectMetadataSpecialCharacters(S3Provider _, string bucket, ISimpleClient client)
    {
        const string value = "!\" #$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
        string key = nameof(PutObjectMetadataSpecialCharacters) + "-" + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectAsync(bucket, key, null, r => r.Metadata.Add("a", value), TestContext.Current.CancellationToken), async _ =>
        {
            using GetObjectResponse response = await client.GetObjectAsync(bucket, key, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(value, response.Metadata!["a"]);
        });
    }

    [Theory]
    [MultipleProvidersWithData(S3Provider.All, 1, 10)]
    public async Task PutObjectMetadata(S3Provider _, string bucket, ISimpleClient client, int count)
    {
        string key = nameof(PutObjectMetadata) + "-" + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectAsync(bucket, key, null, r =>
        {
            for (int i = 0; i < count; i++)
                r.Metadata.Add("mykey" + i, "myvalue" + i);
        }, TestContext.Current.CancellationToken), async _ =>
        {
            using GetObjectResponse response = await client.GetObjectAsync(bucket, key, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(count, response.Metadata!.Count);
            for (int i = 0; i < count; i++)
                Assert.Equal("myvalue" + i, response.Metadata["mykey" + i]);
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task PutObjectMultipleTags(S3Provider _, string bucket, ISimpleClient client)
    {
        string key = nameof(PutObjectMultipleTags) + "-" + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectAsync(bucket, key, null, r =>
        {
            r.Tags.Add("mykey1", "myvalue1");
            r.Tags.Add("mykey2", "myvalue2");
        }, TestContext.Current.CancellationToken), async _ =>
        {
            using GetObjectResponse response = await client.GetObjectAsync(bucket, key, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(2, response.TagCount);
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task PutObjectServerSideEncryptionCustomerKey(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        await AllowSseCustomerKeysAsync(provider, client, bucket);

        byte[] key = new byte[32];
        Random.Shared.NextBytes(key);

        byte[] keyHash = CryptoHelper.Md5Hash(key);

        PutObjectResponse putResp = await client.PutObjectStringAsync(bucket, nameof(PutObjectServerSideEncryptionCustomerKey), "encrypted content", null, r =>
        {
            r.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
            r.SseCustomerKey = key;
            r.SseCustomerKeyMd5 = keyHash;
        }, TestContext.Current.CancellationToken);
        Assert.Equal(200, putResp.StatusCode);
        Assert.Equal(SseCustomerAlgorithm.Aes256, putResp.SseCustomerAlgorithm);
        Assert.Equal(keyHash, putResp.SseCustomerKeyMd5);

        using GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(PutObjectServerSideEncryptionCustomerKey), r =>
        {
            r.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
            r.SseCustomerKey = key;
            r.SseCustomerKeyMd5 = keyHash;
        }, TestContext.Current.CancellationToken);
        Assert.Equal(200, getResp.StatusCode);
        Assert.Equal("encrypted content", await getResp.Content.AsStringAsync(token: TestContext.Current.CancellationToken));
    });

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task PutObjectTooManyTags(S3Provider _, string bucket, ISimpleClient client) => await Assert.ThrowsAsync<InvalidOperationException>(async () => await client.PutObjectAsync(bucket, nameof(PutObjectTooManyTags), null, r =>
    {
        for (int i = 0; i < 11; i++)
            r.Tags.Add(i.ToString(NumberFormatInfo.InvariantInfo), i.ToString(NumberFormatInfo.InvariantInfo));
    }, TestContext.Current.CancellationToken));

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task PutObjectWebsiteRedirect(S3Provider _, string bucket, ISimpleClient client)
    {
        string key = nameof(PutObjectWebsiteRedirect) + "-" + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectAsync(bucket, key, null, r => r.WebsiteRedirectLocation = "https://google.com", TestContext.Current.CancellationToken), async _ =>
        {
            using GetObjectResponse response = await client.GetObjectAsync(bucket, key, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("https://google.com", response.WebsiteRedirectLocation);
        });
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    [SuppressMessage("Security", "CA5350", Justification = "S3 requires this legacy hash for its wire-level checksum; it is not used for password storage or a new signature scheme.")]
    public async Task PutObjectChecksum(S3Provider _, string bucket, ISimpleClient client)
    {
        byte[] data = [1, 2, 3, 4];
        byte[] checksum = SHA1.HashData(data);

        string key = nameof(PutObjectChecksum) + "-" + Guid.NewGuid().ToString("N");
        await TemporaryObject.WithAsync(client, bucket, key, () => client.PutObjectDataAsync(bucket, key, data, r =>
        {
            r.ChecksumAlgorithm = ChecksumAlgorithm.Sha1;
            r.Checksum = checksum;
        }, TestContext.Current.CancellationToken), async put =>
        {
            Assert.Equal(checksum, put.Checksum);
            Assert.Equal(ChecksumType.FullObject, put.ChecksumType);
            using GetObjectResponse response = await client.GetObjectAsync(bucket, key, r => r.EnableChecksum = true, TestContext.Current.CancellationToken);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(ChecksumType.FullObject, response.ChecksumType);
            Assert.Equal(ChecksumAlgorithm.Sha1, response.ChecksumAlgorithm);
            Assert.Equal(checksum, response.Checksum);
        });
    }
}