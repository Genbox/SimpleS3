using System;
using System.Globalization;
using System.Threading.Tasks;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Objects
{
    public class PutObjectTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.AmazonS3, ObjectCannedAcl.AuthenticatedRead, ObjectCannedAcl.AwsExecRead, ObjectCannedAcl.BucketOwnerFullControl, ObjectCannedAcl.BucketOwnerRead, ObjectCannedAcl.Private, ObjectCannedAcl.PublicRead, ObjectCannedAcl.PublicReadWrite)]
        [MultipleProviders(S3Provider.BackBlazeB2, ObjectCannedAcl.Private)]
        [MultipleProviders(S3Provider.GoogleCloudStorage, ObjectCannedAcl.AuthenticatedRead, ObjectCannedAcl.BucketOwnerFullControl, ObjectCannedAcl.BucketOwnerRead, ObjectCannedAcl.Private, ObjectCannedAcl.PublicRead)]
        public async Task PutObjectCannedAcl(S3Provider _, string bucket, ISimpleClient client, ObjectCannedAcl acl)
        {
            PutObjectResponse resp = await client.PutObjectAsync(bucket, $"{nameof(PutObjectCannedAcl)}-{acl}", null, r => r.Acl = acl);
            Assert.Equal(200, resp.StatusCode);
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3, LockMode.Compliance, LockMode.Governance)]
        public async Task PutObjectLockMode(S3Provider _, string bucket, ISimpleClient client, LockMode lockMode)
        {
            DateTimeOffset lockRetainUntil = DateTimeOffset.UtcNow.AddMinutes(1);

            //We add a unique guid to prevent contamination across runs
            string objectKey = $"{nameof(PutObjectLockMode)}-{lockMode}-{Guid.NewGuid()}";

            PutObjectResponse putResp = await client.PutObjectStringAsync(bucket, objectKey, "bla", null, r =>
            {
                r.LockMode = lockMode;
                r.LockRetainUntil = lockRetainUntil;
            }).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            GetObjectResponse getResp = await client.GetObjectAsync(bucket, objectKey).ConfigureAwait(false);
            Assert.Equal(200, getResp.StatusCode);
            Assert.Equal(lockMode, getResp.LockMode);
            Assert.Equal(lockRetainUntil.DateTime, getResp.LockRetainUntil!.Value.DateTime, TimeSpan.FromSeconds(1));
        }

        [Theory]
        [MultipleProviders(S3Provider.All, "NormalFile", "This/Should/Look/Like/Directories/File.txt", "_\\_", "~", "/", " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~/")]
        public async Task PutObjectValidCharacters(S3Provider _, string bucket, ISimpleClient client, string name)
        {
            PutObjectResponse putResp = await client.PutObjectAsync(bucket, name, null).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            GetObjectResponse getResp = await client.GetObjectAsync(bucket, name).ConfigureAwait(false);
            Assert.Equal(200, getResp.StatusCode);
        }

        [Theory]
        [MultipleProviders(S3Provider.All, ".", "\0")]
        public async Task PutObjectInvalidCharacters(S3Provider _, string bucket, ISimpleClient client, string name)
        {
            //These 2 test cases came after an exhaustive search in the whole UTF-16 character space.
            PutObjectResponse putResp = await client.PutObjectAsync(bucket, name, null).ConfigureAwait(false);

            //We use IsSuccess here since different providers return different error codes
            Assert.False(putResp.IsSuccess);
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3, SseAlgorithm.Aes256, SseAlgorithm.AwsKms)]
        [MultipleProviders(S3Provider.BackBlazeB2, SseAlgorithm.Aes256)]
        public async Task PutObjectServerSideEncryption(S3Provider _, string bucket, ISimpleClient client, SseAlgorithm algorithm)
        {
            PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(PutObjectServerSideEncryption), null, r => r.SseAlgorithm = algorithm).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);
            Assert.Equal(algorithm, putResp.SseAlgorithm);

            GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(PutObjectServerSideEncryption)).ConfigureAwait(false);
            Assert.Equal(200, getResp.StatusCode);
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3, StorageClass.Standard, StorageClass.DeepArchive, StorageClass.Glacier, StorageClass.IntelligentTiering, StorageClass.OneZoneIa, StorageClass.ReducedRedundancy, StorageClass.StandardIa)]
        [MultipleProviders(S3Provider.BackBlazeB2 | S3Provider.GoogleCloudStorage, StorageClass.Standard)]
        public async Task PutObjectStorageClass(S3Provider _, string bucket, ISimpleClient client, StorageClass storageClass)
        {
            PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(PutObjectStorageClass) + "-" + storageClass, null, r => r.StorageClass = storageClass).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);
            Assert.Equal(storageClass, putResp.StorageClass);

            if (storageClass is StorageClass.DeepArchive or StorageClass.Glacier)
                return;

            GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(PutObjectStorageClass) + "-" + storageClass).ConfigureAwait(false);
            Assert.Equal(storageClass, getResp.StorageClass);
            Assert.Equal(200, getResp.StatusCode);
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3 | S3Provider.GoogleCloudStorage)]
        public async Task MultiplePermissions(S3Provider _, string bucket, ISimpleClient client)
        {
            PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(MultiplePermissions), null, r =>
            {
                r.AclGrantRead.AddEmail(TestConstants.TestEmail);
                r.AclGrantRead.AddUserId(TestConstants.TestUserId);
                r.AclGrantReadAcp.AddEmail(TestConstants.TestEmail);
                r.AclGrantReadAcp.AddUserId(TestConstants.TestUserId);
                r.AclGrantWriteAcp.AddEmail(TestConstants.TestEmail);
                r.AclGrantWriteAcp.AddUserId(TestConstants.TestUserId);
                r.AclGrantFullControl.AddEmail(TestConstants.TestEmail);
                r.AclGrantFullControl.AddUserId(TestConstants.TestUserId);
            }).ConfigureAwait(false);

            Assert.Equal(200, putResp.StatusCode);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectCacheControl(S3Provider _, string bucket, ISimpleClient client)
        {
            PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(PutObjectCacheControl), null, r => r.CacheControl.Set(CacheControlType.MaxAge, 100)).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(PutObjectCacheControl)).ConfigureAwait(false);
            Assert.Equal(200, getResp.StatusCode);
            Assert.Equal("max-age=100", getResp.CacheControl);
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3 | S3Provider.GoogleCloudStorage)]
        public async Task PutObjectContentProperties(S3Provider _, string bucket, ISimpleClient client)
        {
            PutObjectResponse putResp = await client.PutObjectStringAsync(bucket, nameof(PutObjectContentProperties), "test", null, r =>
            {
                r.ContentDisposition.Set(ContentDispositionType.Attachment, "filename.jpg");
                r.ContentEncoding.Add(ContentEncodingType.Identity);
                r.ContentType.Set(MediaType.Text_plain, Charset.Utf_8);
            }).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(PutObjectContentProperties)).ConfigureAwait(false);
            Assert.Equal(200, getResp.StatusCode);
            Assert.Equal(4, getResp.ContentLength);
            Assert.Equal("attachment; filename*=\"filename.jpg\"", getResp.ContentDisposition);
            Assert.Equal("text/plain; charset=utf-8", getResp.ContentType);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectEtag(S3Provider _, string bucket, ISimpleClient client)
        {
            PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(PutObjectEtag), null).ConfigureAwait(false);

            await client.GetObjectAsync(bucket, nameof(PutObjectEtag), r => r.IfETagMatch.Set(putResp.ETag)).ConfigureAwait(false);

            GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(PutObjectEtag), r => r.IfETagMatch.Set("not the tag you are looking for")).ConfigureAwait(false);
            Assert.Equal(412, getResp.StatusCode);

            await client.GetObjectAsync(bucket, nameof(PutObjectEtag), r => r.IfETagNotMatch.Set("not the tag you are looking for")).ConfigureAwait(false);

            GetObjectResponse getResp2 = await client.GetObjectAsync(bucket, nameof(PutObjectEtag), r => r.IfETagNotMatch.Set(putResp.ETag)).ConfigureAwait(false);
            Assert.Equal(304, getResp2.StatusCode);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectLargeMetadata(S3Provider _, string bucket, ISimpleClient client)
        {
            string value = new string('b', 2047);

            PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(PutObjectMultipleMetadata), null, r =>
            {
                //Amazon ignores the metadata prefix and header separator, so we just need to count key length + value length
                r.Metadata.Add("a", value);
            }).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(PutObjectMultipleMetadata)).ConfigureAwait(false);
            Assert.Equal(200, getResp.StatusCode);
            Assert.Equal(value, getResp.Metadata!["a"]);
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)]
        public async Task PutObjectLegalHold(S3Provider _, string bucket, ISimpleClient client)
        {
            await client.PutObjectStringAsync(bucket, nameof(PutObjectLegalHold), "test", null, r => r.LockLegalHold = true).ConfigureAwait(false);

            GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(PutObjectLegalHold)).ConfigureAwait(false);
            Assert.Equal(200, getResp.StatusCode);
            Assert.True(getResp.LockLegalHold);

            HeadObjectResponse headResp = await client.HeadObjectAsync(bucket, nameof(PutObjectLegalHold)).ConfigureAwait(false);
            Assert.Equal(200, headResp.StatusCode);
            Assert.True(headResp.LockLegalHold);
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3)]
        public async Task PutObjectLifecycle(S3Provider _, string bucket, ISimpleClient client)
        {
            PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(PutObjectLifecycle), null).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            //Test lifecycle expiration (yes, we add 2 days. I don't know why Amazon works like this)
            Assert.Equal(DateTime.UtcNow.AddDays(2).Date, putResp.LifeCycleExpiresOn!.Value.UtcDateTime.Date);
            Assert.Equal("ExpireAll", putResp.LifeCycleRuleId);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectMetadataSpecialCharacters(S3Provider _, string bucket, ISimpleClient client)
        {
            PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(PutObjectMultipleMetadata), null, r => r.Metadata.Add("a", "!\" #$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~"));
            Assert.Equal(200, putResp.StatusCode);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectMultipleMetadata(S3Provider _, string bucket, ISimpleClient client)
        {
            PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(PutObjectMultipleMetadata), null, r =>
            {
                for (int i = 0; i < 10; i++)
                {
                    r.Metadata.Add("mykey" + i, "myvalue" + i);
                }
            }).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(PutObjectMultipleMetadata)).ConfigureAwait(false);
            Assert.Equal(200, getResp.StatusCode);
            Assert.Equal(10, getResp.Metadata!.Count);
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3)]
        public async Task PutObjectMultipleTags(S3Provider _, string bucket, ISimpleClient client)
        {
            PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(PutObjectMultipleTags), null, r =>
            {
                r.Tags.Add("mykey1", "myvalue1");
                r.Tags.Add("mykey2", "myvalue2");
            }).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(PutObjectMultipleTags)).ConfigureAwait(false);
            Assert.Equal(200, getResp.StatusCode);
            Assert.Equal(2, getResp.TagCount);
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2)] //Google does not seem to support CacheControl
        public async Task PutObjectResponseHeaders(S3Provider _, string bucket, ISimpleClient client)
        {
            //Upload a file for tests
            PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(PutObjectResponseHeaders), null).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(PutObjectResponseHeaders), req =>
            {
                req.ResponseCacheControl.Set(CacheControlType.MaxAge, 42);
                req.ResponseContentDisposition.Set(ContentDispositionType.Attachment, "filename.txt");
                req.ResponseContentEncoding.Add(ContentEncodingType.Gzip);
                req.ResponseContentLanguage.Add("da-DK");
                req.ResponseContentType.Set("text/html", "utf-8");
                req.ResponseExpires = DateTimeOffset.UtcNow;
            }).ConfigureAwait(false);
            Assert.Equal(200, getResp.StatusCode);

            Assert.Equal("max-age=42", getResp.CacheControl);
            Assert.Equal("attachment; filename=\"filename.txt\"", getResp.ContentDisposition);
            Assert.Equal("gzip", getResp.ContentEncoding);
            Assert.Equal("da-DK", getResp.ContentLanguage);
            Assert.Equal("text/html; charset=utf-8", getResp.ContentType);
            Assert.Equal(DateTime.UtcNow, getResp.ExpiresOn!.Value.DateTime, TimeSpan.FromSeconds(5));
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3)]
        public async Task PutObjectServerSideEncryptionCustomerKey(S3Provider _, string bucket, ISimpleClient client)
        {
            byte[] key = new byte[32];
            new Random(42).NextBytes(key);

            byte[] keyHash = CryptoHelper.Md5Hash(key);

            PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(PutObjectServerSideEncryptionCustomerKey), null, r =>
            {
                r.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                r.SseCustomerKey = key;
                r.SseCustomerKeyMd5 = keyHash;
            }).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);
            Assert.Equal(SseCustomerAlgorithm.Aes256, putResp.SseCustomerAlgorithm);
            Assert.Equal(keyHash, putResp.SseCustomerKeyMd5);

            GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(PutObjectServerSideEncryptionCustomerKey), r =>
            {
                r.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                r.SseCustomerKey = key;
                r.SseCustomerKeyMd5 = keyHash;
            }).ConfigureAwait(false);
            Assert.Equal(200, getResp.StatusCode);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectSingleMetadata(S3Provider _, string bucket, ISimpleClient client)
        {
            PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(PutObjectSingleMetadata), null, r => r.Metadata.Add("mykey", "myvalue")).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(PutObjectSingleMetadata)).ConfigureAwait(false);
            Assert.Equal(200, getResp.StatusCode);
            Assert.Equal("myvalue", getResp.Metadata!["mykey"]);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task PutObjectTooManyTags(S3Provider _, string bucket, ISimpleClient client)
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await client.PutObjectAsync(bucket, nameof(PutObjectTooManyTags), null, r =>
            {
                for (int i = 0; i < 51; i++)
                {
                    r.Tags.Add(i.ToString(NumberFormatInfo.InvariantInfo), i.ToString(NumberFormatInfo.InvariantInfo));
                }
            }).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3)]
        public async Task PutObjectWebsiteRedirect(S3Provider _, string bucket, ISimpleClient client)
        {
            PutObjectResponse putResp = await client.PutObjectAsync(bucket, nameof(PutObjectWebsiteRedirect), null, r => r.WebsiteRedirectLocation = "https://google.com").ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            GetObjectResponse resp = await client.GetObjectAsync(bucket, nameof(PutObjectWebsiteRedirect)).ConfigureAwait(false);
            Assert.Equal(200, resp.StatusCode);
            Assert.Equal("https://google.com", resp.WebsiteRedirectLocation);
        }
    }
}