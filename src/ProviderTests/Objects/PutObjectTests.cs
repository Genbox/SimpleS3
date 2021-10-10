using System;
using System.Globalization;
using System.Threading.Tasks;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Objects
{
    public class PutObjectTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All)]
        [InlineData(ObjectCannedAcl.AuthenticatedRead)]
        [InlineData(ObjectCannedAcl.AwsExecRead)]
        [InlineData(ObjectCannedAcl.BucketOwnerFullControl)]
        [InlineData(ObjectCannedAcl.BucketOwnerRead)]
        [InlineData(ObjectCannedAcl.Private)]
        [InlineData(ObjectCannedAcl.PublicRead)]
        [InlineData(ObjectCannedAcl.PublicReadWrite)]
        public Task PutObjectCannedAcl(ObjectCannedAcl acl, IProfile profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            return client.PutObjectAsync(bucketName, $"{nameof(PutObjectCannedAcl)}-{acl}", null, req => req.Acl = acl);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        [InlineData(LockMode.Compliance)]
        [InlineData(LockMode.Governance)]
        public async Task PutObjectLockMode(LockMode lockMode, IProfile profile, ISimpleClient client)
        {
            DateTimeOffset lockRetainUntil = DateTimeOffset.UtcNow.AddMinutes(1);

            //We add a unique guid to prevent contamination across runs
            string objectKey = $"{nameof(PutObjectLockMode)}-{lockMode}-{Guid.NewGuid()}";
            string bucketName = GetTestBucket(profile);

            await client.PutObjectAsync(bucketName, objectKey, null, req =>
            {
                req.LockMode = lockMode;
                req.LockRetainUntil = lockRetainUntil;
            }).ConfigureAwait(false);

            GetObjectResponse resp = await client.GetObjectAsync(bucketName, objectKey).ConfigureAwait(false);
            Assert.Equal(lockMode, resp.LockMode);
            Assert.Equal(lockRetainUntil.DateTime, resp.LockRetainUntil!.Value.DateTime, TimeSpan.FromSeconds(1));
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        [InlineData("NormalFile")]
        [InlineData("This/Should/Look/Like/Directories/File.txt")]
        [InlineData("_\\_")]
        [InlineData("~")]
        [InlineData("/")]
        [InlineData(" !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~/")]
        public async Task PutObjectValidCharacters(string name, IProfile profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            PutObjectResponse putResp = await client.PutObjectAsync(bucketName, name, null).ConfigureAwait(false);
            Assert.True(putResp.IsSuccess);

            GetObjectResponse getResp = await client.GetObjectAsync(bucketName, name).ConfigureAwait(false);
            Assert.True(getResp.IsSuccess);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        [InlineData(".")]
        [InlineData("\0")]
        public async Task PutObjectInvalidCharacters(string name, IProfile profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);

            //These 2 test cases came after an exhaustive search in the whole UTF-16 character space.
            await client.PutObjectAsync(bucketName, name, null).ConfigureAwait(false);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        [InlineData(SseAlgorithm.Aes256)]
        [InlineData(SseAlgorithm.AwsKms)]
        public async Task PutObjectServerSideEncryption(SseAlgorithm algorithm, IProfile profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            PutObjectResponse resp = await client.PutObjectAsync(bucketName, nameof(PutObjectServerSideEncryption), null, request => request.SseAlgorithm = algorithm).ConfigureAwait(false);
            Assert.Equal(algorithm, resp.SseAlgorithm);

            await client.GetObjectAsync(bucketName, nameof(PutObjectServerSideEncryption)).ConfigureAwait(false);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        [InlineData(StorageClass.Standard, true)]
        [InlineData(StorageClass.DeepArchive, false)]
        [InlineData(StorageClass.Glacier, false)]
        [InlineData(StorageClass.IntelligentTiering, true)]
        [InlineData(StorageClass.OneZoneIa, true)]
        [InlineData(StorageClass.ReducedRedundancy, true)]
        [InlineData(StorageClass.StandardIa, true)]
        public async Task PutObjectStorageClass(StorageClass storageClass, bool canDownload, IProfile profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            PutObjectResponse putResp = await client.PutObjectAsync(bucketName, nameof(PutObjectStorageClass) + "-" + storageClass, null, req => req.StorageClass = storageClass).ConfigureAwait(false);
            Assert.Equal(storageClass, putResp.StorageClass);

            if (canDownload)
            {
                GetObjectResponse getResp = await client.GetObjectAsync(bucketName, nameof(PutObjectStorageClass) + "-" + storageClass).ConfigureAwait(false);
                Assert.Equal(storageClass, getResp.StorageClass);
            }
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task MultiplePermissions(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            await client.PutObjectAsync(bucketName, nameof(MultiplePermissions), null, request =>
            {
                request.AclGrantRead.AddEmail(TestConstants.TestEmail);
                request.AclGrantRead.AddUserId(TestConstants.TestUserId);
                request.AclGrantReadAcp.AddEmail(TestConstants.TestEmail);
                request.AclGrantReadAcp.AddUserId(TestConstants.TestUserId);
                request.AclGrantWriteAcp.AddEmail(TestConstants.TestEmail);
                request.AclGrantWriteAcp.AddUserId(TestConstants.TestUserId);
                request.AclGrantFullControl.AddEmail(TestConstants.TestEmail);
                request.AclGrantFullControl.AddUserId(TestConstants.TestUserId);
            }).ConfigureAwait(false);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectCacheControl(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            await client.PutObjectAsync(bucketName, nameof(PutObjectCacheControl), null, req => req.CacheControl.Set(CacheControlType.MaxAge, 100)).ConfigureAwait(false);

            GetObjectResponse resp = await client.GetObjectAsync(bucketName, nameof(PutObjectCacheControl)).ConfigureAwait(false);
            Assert.Equal("max-age=100", resp.CacheControl);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectContentProperties(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            await client.PutObjectAsync(bucketName, nameof(PutObjectContentProperties), null, req =>
            {
                req.ContentDisposition.Set(ContentDispositionType.Attachment, "filename.jpg");
                req.ContentEncoding.Add(ContentEncodingType.Identity);
                req.ContentType.Set(MediaType.Text_plain, Charset.Utf_8);
            }).ConfigureAwait(false);

            GetObjectResponse resp = await client.GetObjectAsync(bucketName, nameof(PutObjectContentProperties)).ConfigureAwait(false);
            Assert.Equal(4, resp.ContentLength);
            Assert.Equal("attachment; filename*=\"filename.jpg\"", resp.ContentDisposition);
            Assert.Equal("text/plain; charset=utf-8", resp.ContentType);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectEtag(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            PutObjectResponse putResp = await client.PutObjectAsync(bucketName, nameof(PutObjectEtag), null).ConfigureAwait(false);

            await client.GetObjectAsync(bucketName, nameof(PutObjectEtag), req => req.IfETagMatch.Set(putResp.ETag)).ConfigureAwait(false);

            GetObjectResponse getResp = await client.GetObjectAsync(bucketName, nameof(PutObjectEtag), req => req.IfETagMatch.Set("not the tag you are looking for")).ConfigureAwait(false);
            Assert.Equal(412, getResp.StatusCode);

            await client.GetObjectAsync(bucketName, nameof(PutObjectEtag), req => req.IfETagNotMatch.Set("not the tag you are looking for")).ConfigureAwait(false);

            GetObjectResponse getResp2 = await client.GetObjectAsync(bucketName, nameof(PutObjectEtag), req => req.IfETagNotMatch.Set(putResp.ETag)).ConfigureAwait(false);
            Assert.Equal(304, getResp2.StatusCode);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectLargeMetadata(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            string value = new string('b', 2047);

            await client.PutObjectAsync(bucketName, nameof(PutObjectMultipleMetadata), null, req =>
            {
                //Amazon ignores the metadata prefix and header separator, so we just need to count key length + value length
                req.Metadata.Add("a", value);
            }).ConfigureAwait(false);

            GetObjectResponse gResp = await client.GetObjectAsync(bucketName, nameof(PutObjectMultipleMetadata)).ConfigureAwait(false);
            Assert.Equal(value, gResp.Metadata!["a"]);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectLegalHold(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            await client.PutObjectAsync(bucketName, nameof(PutObjectLegalHold), null, req => req.LockLegalHold = true).ConfigureAwait(false);

            GetObjectResponse getResp = await client.GetObjectAsync(bucketName, nameof(PutObjectLegalHold)).ConfigureAwait(false);
            Assert.True(getResp.LockLegalHold);

            HeadObjectResponse headResp = await client.HeadObjectAsync(bucketName, nameof(PutObjectLegalHold)).ConfigureAwait(false);
            Assert.True(headResp.LockLegalHold);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectLifecycle(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            PutObjectResponse putResp = await client.PutObjectAsync(bucketName, nameof(PutObjectLifecycle), null).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            //Test lifecycle expiration (yes, we add 2 days. I don't know why Amazon works like this)
            Assert.Equal(DateTime.UtcNow.AddDays(2).Date, putResp.LifeCycleExpiresOn!.Value.UtcDateTime.Date);
            Assert.Equal("ExpireAll", putResp.LifeCycleRuleId);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public Task PutObjectMetadataSpecialCharacters(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            return client.PutObjectAsync(bucketName, nameof(PutObjectMultipleMetadata), null, req => req.Metadata.Add("a", "!\" #$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~"));
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectMultipleMetadata(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            await client.PutObjectAsync(bucketName, nameof(PutObjectMultipleMetadata), null, req =>
            {
                for (int i = 0; i < 10; i++)
                {
                    req.Metadata.Add("mykey" + i, "myvalue" + i);
                }
            }).ConfigureAwait(false);

            GetObjectResponse resp = await client.GetObjectAsync(bucketName, nameof(PutObjectMultipleMetadata)).ConfigureAwait(false);
            Assert.Equal(10, resp.Metadata!.Count);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectMultipleTags(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            await client.PutObjectAsync(bucketName, nameof(PutObjectMultipleTags), null, req =>
            {
                req.Tags.Add("mykey1", "myvalue1");
                req.Tags.Add("mykey2", "myvalue2");
            }).ConfigureAwait(false);

            GetObjectResponse gResp = await client.GetObjectAsync(bucketName, nameof(PutObjectMultipleTags)).ConfigureAwait(false);
            Assert.Equal(2, gResp.TagCount);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectResponseHeaders(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);

            //Upload a file for tests
            await client.PutObjectAclAsync(bucketName, nameof(PutObjectResponseHeaders)).ConfigureAwait(false);

            GetObjectResponse resp = await client.GetObjectAsync(bucketName, nameof(PutObjectResponseHeaders), req =>
            {
                req.ResponseCacheControl.Set(CacheControlType.MaxAge, 42);
                req.ResponseContentDisposition.Set(ContentDispositionType.Attachment, "filename.txt");
                req.ResponseContentEncoding.Add(ContentEncodingType.Gzip);
                req.ResponseContentLanguage.Add("da-DK");
                req.ResponseContentType.Set("text/html", "utf-8");
                req.ResponseExpires = DateTimeOffset.UtcNow;
            }).ConfigureAwait(false);

            Assert.Equal("max-age=42", resp.CacheControl);
            Assert.Equal("attachment; filename=\"filename.txt\"", resp.ContentDisposition);
            Assert.Equal("gzip", resp.ContentEncoding);
            Assert.Equal("da-DK", resp.ContentLanguage);
            Assert.Equal("text/html; charset=utf-8", resp.ContentType);
            Assert.Equal(DateTime.UtcNow, resp.ExpiresOn!.Value.DateTime, TimeSpan.FromSeconds(5));
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectServerSideEncryptionCustomerKey(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            byte[] key = new byte[32];
            new Random(42).NextBytes(key);

            byte[] keyHash = CryptoHelper.Md5Hash(key);
            string bucketName = GetTestBucket(profile);

            PutObjectResponse resp = await client.PutObjectAsync(bucketName, nameof(PutObjectServerSideEncryptionCustomerKey), null, req =>
            {
                req.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                req.SseCustomerKey = key;
                req.SseCustomerKeyMd5 = keyHash;
            }).ConfigureAwait(false);

            Assert.Equal(SseCustomerAlgorithm.Aes256, resp.SseCustomerAlgorithm);
            Assert.Equal(keyHash, resp.SseCustomerKeyMd5);

            await client.GetObjectAsync(bucketName, nameof(PutObjectServerSideEncryptionCustomerKey), req =>
            {
                req.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                req.SseCustomerKey = key;
                req.SseCustomerKeyMd5 = keyHash;
            }).ConfigureAwait(false);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectSingleMetadata(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            await client.PutObjectAsync(bucketName, nameof(PutObjectSingleMetadata), null, req => req.Metadata.Add("mykey", "myvalue")).ConfigureAwait(false);

            GetObjectResponse resp = await client.GetObjectAsync(bucketName, nameof(PutObjectSingleMetadata)).ConfigureAwait(false);
            Assert.Equal("myvalue", resp.Metadata!["mykey"]);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectTooManyTags(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await client.PutObjectAsync(bucketName, nameof(PutObjectTooManyTags), null, request =>
            {
                for (int i = 0; i < 51; i++)
                {
                    request.Tags.Add(i.ToString(NumberFormatInfo.InvariantInfo), i.ToString(NumberFormatInfo.InvariantInfo));
                }
            }).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectWebsiteRedirect(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            await client.PutObjectAsync(bucketName, nameof(PutObjectWebsiteRedirect), null, req => req.WebsiteRedirectLocation = "https://google.com").ConfigureAwait(false);

            GetObjectResponse resp = await client.GetObjectAsync(bucketName, nameof(PutObjectWebsiteRedirect)).ConfigureAwait(false);
            Assert.Equal("https://google.com", resp.WebsiteRedirectLocation);
            Assert.Equal(200, resp.StatusCode);
        }
    }
}