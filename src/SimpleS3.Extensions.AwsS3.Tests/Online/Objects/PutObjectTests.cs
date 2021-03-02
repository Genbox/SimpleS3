using System;
using System.Globalization;
using System.Threading.Tasks;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.TestBase;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Extensions.AwsS3.Tests.Online.Objects
{
    public class PutObjectTests : AwsTestBase
    {
        public PutObjectTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        [Theory]
        [InlineData(ObjectCannedAcl.AuthenticatedRead)]
        [InlineData(ObjectCannedAcl.AwsExecRead)]
        [InlineData(ObjectCannedAcl.BucketOwnerFullControl)]
        [InlineData(ObjectCannedAcl.BucketOwnerRead)]
        [InlineData(ObjectCannedAcl.Private)]
        [InlineData(ObjectCannedAcl.PublicRead)]
        [InlineData(ObjectCannedAcl.PublicReadWrite)]
        public Task PutObjectCannedAcl(ObjectCannedAcl acl)
        {
            return UploadAsync($"{nameof(PutObjectCannedAcl)}-{acl}", req => req.Acl = acl);
        }

        [Theory]
        [InlineData(LockMode.Compliance)]
        [InlineData(LockMode.Governance)]
        public async Task PutObjectLockMode(LockMode lockMode)
        {
            DateTimeOffset lockRetainUntil = DateTimeOffset.UtcNow.AddMinutes(1);

            //We add a unique guid to prevent contamination across runs
            string objectKey = $"{nameof(PutObjectLockMode)}-{lockMode}-{Guid.NewGuid()}";

            await UploadAsync(objectKey, req =>
            {
                req.LockMode = lockMode;
                req.LockRetainUntil = lockRetainUntil;
            }).ConfigureAwait(false);

            GetObjectResponse resp = await AssertAsync(objectKey).ConfigureAwait(false);
            Assert.Equal(lockMode, resp.LockMode);
            Assert.Equal(lockRetainUntil.DateTime, resp.LockRetainUntil!.Value.DateTime, TimeSpan.FromSeconds(1));
        }

        [Theory]
        [InlineData("NormalFile")]
        [InlineData("This/Should/Look/Like/Directories/File.txt")]
        [InlineData("_\\_")]
        [InlineData("~")]
        [InlineData("/")]
        [InlineData(" !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~/")]
        public async Task PutObjectValidCharacters(string name)
        {
            PutObjectResponse putResp = await UploadAsync(BucketName, name).ConfigureAwait(false);
            Assert.True(putResp.IsSuccess);

            GetObjectResponse getResp = await ObjectClient.GetObjectAsync(BucketName, name).ConfigureAwait(false);
            Assert.True(getResp.IsSuccess);
        }

        [Theory]
        [InlineData(".")]
        [InlineData("\0")]
        public async Task PutObjectInvalidCharacters(string name)
        {
            //These 2 test cases came after an exhaustive search in the whole UTF-16 character space.
            await UploadAsync(BucketName, name, assumeSuccess: false).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(SseAlgorithm.Aes256)]
        [InlineData(SseAlgorithm.AwsKms)]
        public async Task PutObjectServerSideEncryption(SseAlgorithm algorithm)
        {
            PutObjectResponse resp = await UploadAsync(nameof(PutObjectServerSideEncryption), request => request.SseAlgorithm = algorithm).ConfigureAwait(false);
            Assert.Equal(algorithm, resp.SseAlgorithm);

            await AssertAsync(nameof(PutObjectServerSideEncryption)).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(StorageClass.Standard, true)]
        [InlineData(StorageClass.DeepArchive, false)]
        [InlineData(StorageClass.Glacier, false)]
        [InlineData(StorageClass.IntelligentTiering, true)]
        [InlineData(StorageClass.OneZoneIa, true)]
        [InlineData(StorageClass.ReducedRedundancy, true)]
        [InlineData(StorageClass.StandardIa, true)]
        public async Task PutObjectStorageClass(StorageClass storageClass, bool canDownload)
        {
            PutObjectResponse putResp = await UploadAsync(nameof(PutObjectStorageClass) + "-" + storageClass, req => req.StorageClass = storageClass).ConfigureAwait(false);
            Assert.Equal(storageClass, putResp.StorageClass);

            if (canDownload)
            {
                GetObjectResponse getResp = await AssertAsync(nameof(PutObjectStorageClass) + "-" + storageClass).ConfigureAwait(false);
                Assert.Equal(storageClass, getResp.StorageClass);
            }
        }

        [Fact]
        public async Task MultiplePermissions()
        {
            await UploadAsync(nameof(MultiplePermissions), request =>
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

        [Fact]
        public async Task PutObjectCacheControl()
        {
            await UploadAsync(nameof(PutObjectCacheControl), req => req.CacheControl.Set(CacheControlType.MaxAge, 100)).ConfigureAwait(false);

            GetObjectResponse resp = await AssertAsync(nameof(PutObjectCacheControl)).ConfigureAwait(false);
            Assert.Equal("max-age=100", resp.CacheControl);
        }

        [Fact]
        public async Task PutObjectContentProperties()
        {
            await UploadAsync(nameof(PutObjectContentProperties), req =>
            {
                req.ContentDisposition.Set(ContentDispositionType.Attachment, "filename.jpg");
                req.ContentEncoding.Add(ContentEncodingType.Identity);
                req.ContentType.Set(MediaType.Text_plain, Charset.Utf_8);
            }).ConfigureAwait(false);

            GetObjectResponse resp = await AssertAsync(nameof(PutObjectContentProperties)).ConfigureAwait(false);
            Assert.Equal(4, resp.ContentLength);
            Assert.Equal("attachment; filename*=\"filename.jpg\"", resp.ContentDisposition);
            Assert.Equal("text/plain; charset=utf-8", resp.ContentType);
        }

        [Fact]
        public async Task PutObjectEtag()
        {
            PutObjectResponse putResp = await UploadAsync(nameof(PutObjectEtag)).ConfigureAwait(false);

            await AssertAsync(nameof(PutObjectEtag), req => req.IfETagMatch.Set(putResp.ETag)).ConfigureAwait(false);

            GetObjectResponse getResp = await AssertAsync(nameof(PutObjectEtag), req => req.IfETagMatch.Set("not the tag you are looking for"), false).ConfigureAwait(false);
            Assert.Equal(412, getResp.StatusCode);

            await AssertAsync(nameof(PutObjectEtag), req => req.IfETagNotMatch.Set("not the tag you are looking for")).ConfigureAwait(false);

            GetObjectResponse getResp2 = await AssertAsync(nameof(PutObjectEtag), req => req.IfETagNotMatch.Set(putResp.ETag), false).ConfigureAwait(false);
            Assert.Equal(304, getResp2.StatusCode);
        }

        [Fact]
        public async Task PutObjectLargeMetadata()
        {
            string value = new string('b', 2047);

            await UploadAsync(nameof(PutObjectMultipleMetadata), req =>
            {
                //Amazon ignores the metadata prefix and header separator, so we just need to count key length + value length
                req.Metadata.Add("a", value);
            }).ConfigureAwait(false);

            GetObjectResponse gResp = await AssertAsync(nameof(PutObjectMultipleMetadata)).ConfigureAwait(false);
            Assert.Equal(value, gResp.Metadata!["a"]);
        }

        [Fact]
        public async Task PutObjectLegalHold()
        {
            await UploadAsync(nameof(PutObjectLegalHold), req => req.LockLegalHold = true).ConfigureAwait(false);

            GetObjectResponse getResp = await AssertAsync(nameof(PutObjectLegalHold)).ConfigureAwait(false);
            Assert.True(getResp.LockLegalHold);

            HeadObjectResponse headResp = await ObjectClient.HeadObjectAsync(BucketName, nameof(PutObjectLegalHold)).ConfigureAwait(false);
            Assert.True(headResp.LockLegalHold);
        }

        [Fact]
        public async Task PutObjectLifecycle()
        {
            PutObjectResponse putResp = await UploadAsync(nameof(PutObjectLifecycle)).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            //Test lifecycle expiration (yes, we add 2 days. I don't know why Amazon works like this)
            Assert.Equal(DateTime.UtcNow.AddDays(2).Date, putResp.LifeCycleExpiresOn!.Value.UtcDateTime.Date);
            Assert.Equal("ExpireAll", putResp.LifeCycleRuleId);
        }

        [Fact]
        public Task PutObjectMetadataSpecialCharacters()
        {
            return UploadAsync(nameof(PutObjectMultipleMetadata), req => req.Metadata.Add("a", "!\" #$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~"));
        }

        [Fact]
        public async Task PutObjectMultipleMetadata()
        {
            await UploadAsync(nameof(PutObjectMultipleMetadata), req =>
            {
                for (int i = 0; i < 10; i++)
                {
                    req.Metadata.Add("mykey" + i, "myvalue" + i);
                }
            }).ConfigureAwait(false);

            GetObjectResponse resp = await AssertAsync(nameof(PutObjectMultipleMetadata)).ConfigureAwait(false);
            Assert.Equal(10, resp.Metadata!.Count);
        }

        [Fact]
        public async Task PutObjectMultipleTags()
        {
            await UploadAsync(nameof(PutObjectMultipleTags), req =>
            {
                req.Tags.Add("mykey1", "myvalue1");
                req.Tags.Add("mykey2", "myvalue2");
            }).ConfigureAwait(false);

            GetObjectResponse gResp = await AssertAsync(nameof(PutObjectMultipleTags)).ConfigureAwait(false);
            Assert.Equal(2, gResp.TagCount);
        }

        [Fact(Skip = "Require a setup of another AWS account with 'Requester pays' setup")]
        public async Task PutObjectRequestPayer()
        {
            PutObjectResponse putResp = await ObjectClient.PutObjectAsync(BucketName, nameof(PutObjectRequestPayer), null, req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(putResp.RequestCharged);

            GetObjectResponse getResp = await ObjectClient.GetObjectAsync(BucketName, nameof(PutObjectRequestPayer), req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(getResp.RequestCharged);
        }

        [Fact]
        public async Task PutObjectResponseHeaders()
        {
            //Upload a file for tests
            await UploadAsync(nameof(PutObjectResponseHeaders)).ConfigureAwait(false);

            GetObjectResponse resp = await ObjectClient.GetObjectAsync(BucketName, nameof(PutObjectResponseHeaders), req =>
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

        [Fact]
        public async Task PutObjectServerSideEncryptionCustomerKey()
        {
            byte[] key = new byte[32];
            new Random(42).NextBytes(key);

            byte[] keyHash = CryptoHelper.Md5Hash(key);

            PutObjectResponse resp = await UploadAsync(nameof(PutObjectServerSideEncryptionCustomerKey), req =>
            {
                req.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                req.SseCustomerKey = key;
                req.SseCustomerKeyMd5 = keyHash;
            }).ConfigureAwait(false);

            Assert.Equal(SseCustomerAlgorithm.Aes256, resp.SseCustomerAlgorithm);
            Assert.Equal(keyHash, resp.SseCustomerKeyMd5);

            await AssertAsync(nameof(PutObjectServerSideEncryptionCustomerKey), req =>
            {
                req.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                req.SseCustomerKey = key;
                req.SseCustomerKeyMd5 = keyHash;
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task PutObjectSingleMetadata()
        {
            await UploadAsync(nameof(PutObjectSingleMetadata), req => req.Metadata.Add("mykey", "myvalue")).ConfigureAwait(false);

            GetObjectResponse resp = await AssertAsync(nameof(PutObjectSingleMetadata)).ConfigureAwait(false);
            Assert.Equal("myvalue", resp.Metadata!["mykey"]);
        }

        [Fact]
        public async Task PutObjectTooManyTags()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await UploadAsync(BucketName, nameof(PutObjectTooManyTags), request =>
            {
                for (int i = 0; i < 51; i++)
                {
                    request.Tags.Add(i.ToString(NumberFormatInfo.InvariantInfo), i.ToString(NumberFormatInfo.InvariantInfo));
                }
            }).ConfigureAwait(false)).ConfigureAwait(false);
        }

        [Fact]
        public async Task PutObjectWebsiteRedirect()
        {
            await UploadAsync(nameof(PutObjectWebsiteRedirect), req => req.WebsiteRedirectLocation = "https://google.com").ConfigureAwait(false);

            GetObjectResponse resp = await AssertAsync(nameof(PutObjectWebsiteRedirect)).ConfigureAwait(false);
            Assert.Equal("https://google.com", resp.WebsiteRedirectLocation);
            Assert.Equal(200, resp.StatusCode);
        }
    }
}