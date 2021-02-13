using System;
using System.Threading.Tasks;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.TestBase;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Extensions.AwsS3.Tests.Online.Transfer
{
    public class UploadTests : AwsTestBase
    {
        public UploadTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        [Theory]
        [InlineData(LockMode.Compliance)]
        [InlineData(LockMode.Governance)]
        public async Task UploadWithLock(LockMode lockMode)
        {
            DateTimeOffset lockRetainUntil = DateTimeOffset.UtcNow.AddMinutes(1);

            //We add a unique guid to prevent contamination across runs
            string objectKey = $"{nameof(UploadWithLock)}-{lockMode}-{Guid.NewGuid()}";

            await UploadTransferAsync(objectKey, upload => upload.WithLock(lockMode, lockRetainUntil)).ConfigureAwait(false);

            GetObjectResponse resp = await AssertAsync(objectKey).ConfigureAwait(false);
            Assert.Equal(lockMode, resp.LockMode);
            Assert.Equal(lockRetainUntil.DateTime, resp.LockRetainUntil!.Value.DateTime, TimeSpan.FromSeconds(1));
        }

        [Theory]
        [InlineData(ObjectCannedAcl.AuthenticatedRead)]
        [InlineData(ObjectCannedAcl.AwsExecRead)]
        [InlineData(ObjectCannedAcl.BucketOwnerFullControl)]
        [InlineData(ObjectCannedAcl.BucketOwnerRead)]
        [InlineData(ObjectCannedAcl.Private)]
        [InlineData(ObjectCannedAcl.PublicRead)]
        [InlineData(ObjectCannedAcl.PublicReadWrite)]
        public Task UploadWithCannedAcl(ObjectCannedAcl acl)
        {
            return UploadTransferAsync($"{nameof(UploadWithCannedAcl)}-{acl}", upload => upload.WithAccessControl(acl));
        }

        [Theory]
        [InlineData(StorageClass.Standard, true)]
        [InlineData(StorageClass.DeepArchive, false)]
        [InlineData(StorageClass.Glacier, false)]
        [InlineData(StorageClass.IntelligentTiering, true)]
        [InlineData(StorageClass.OneZoneIa, true)]
        [InlineData(StorageClass.ReducedRedundancy, true)]
        [InlineData(StorageClass.StandardIa, true)]
        public async Task UploadStorageClass(StorageClass storageClass, bool canDownload)
        {
            PutObjectResponse pResp = await UploadTransferAsync(nameof(UploadStorageClass) + "-" + storageClass, upload => upload.WithStorageClass(storageClass)).ConfigureAwait(false);
            Assert.Equal(storageClass, pResp.StorageClass);

            if (canDownload)
            {
                GetObjectResponse gResp = await AssertAsync(nameof(UploadStorageClass) + "-" + storageClass).ConfigureAwait(false);
                Assert.Equal(storageClass, gResp.StorageClass);
            }
        }

        [Fact]
        public async Task UploadCacheControl()
        {
            await UploadTransferAsync(nameof(UploadCacheControl), upload => upload.WithCacheControl(CacheControlType.MaxAge, 100)).ConfigureAwait(false);

            GetObjectResponse gResp = await AssertAsync(nameof(UploadCacheControl)).ConfigureAwait(false);
            Assert.Equal("max-age=100", gResp.CacheControl);
        }

        [Fact]
        public async Task UploadContentProperties()
        {
            await UploadTransferAsync(nameof(UploadContentProperties), upload =>
            {
                upload.WithContentDisposition(ContentDispositionType.Attachment, "filename.jpg");
                upload.WithContentEncoding(ContentEncodingType.Identity);
                upload.WithContentType("text/html", "utf-8");
            }).ConfigureAwait(false);

            GetObjectResponse resp = await AssertAsync(nameof(UploadContentProperties)).ConfigureAwait(false);
            Assert.Equal(4, resp.ContentLength);
            Assert.Equal("attachment; filename*=\"filename.jpg\"", resp.ContentDisposition);
            Assert.Equal("text/html; charset=utf-8", resp.ContentType);
        }

        [Fact]
        public async Task UploadEtag()
        {
            PutObjectResponse putResp = await UploadTransferAsync(nameof(UploadEtag)).ConfigureAwait(false);

            await AssertTransferAsync(nameof(UploadEtag), down => down.WithEtagConditional(putResp.ETag)).ConfigureAwait(false);

            GetObjectResponse getResp = await AssertTransferAsync(nameof(UploadEtag), down => down.WithEtagConditional("not the tag you are looking for"), false).ConfigureAwait(false);
            Assert.Equal(412, getResp.StatusCode);

            await AssertTransferAsync(nameof(UploadEtag), down => down.WithEtagConditional(null, "not the tag you are looking for")).ConfigureAwait(false);

            GetObjectResponse getResp2 = await AssertTransferAsync(nameof(UploadEtag), down => down.WithEtagConditional(null, putResp.ETag), false).ConfigureAwait(false);
            Assert.Equal(304, getResp2.StatusCode);
        }

        [Fact]
        public async Task UploadMetadata()
        {
            await UploadTransferAsync(nameof(UploadMetadata), upload =>
            {
                for (int i = 0; i < 10; i++)
                {
                    upload.WithMetadata("mykey" + i, "myvalue" + i);
                }
            }).ConfigureAwait(false);

            GetObjectResponse gResp = await AssertAsync(nameof(UploadMetadata)).ConfigureAwait(false);
            Assert.Equal(10, gResp.Metadata!.Count);
        }

        [Fact]
        public Task UploadMultiplePermissions()
        {
            ObjectAclBuilder acl = new ObjectAclBuilder();
            acl.AddEmail(TestConstants.TestEmail, ObjectPermissions.Read | ObjectPermissions.ReadAcl | ObjectPermissions.WriteAcl | ObjectPermissions.FullControl);
            acl.AddUserId(TestConstants.TestUserId, ObjectPermissions.Read | ObjectPermissions.ReadAcl | ObjectPermissions.WriteAcl | ObjectPermissions.FullControl);

            return UploadTransferAsync(nameof(UploadMultiplePermissions), upload => upload.WithAccessControl(acl));
        }

        [Fact]
        public async Task UploadMultipleTags()
        {
            await UploadTransferAsync(nameof(UploadMultipleTags), upload =>
            {
                upload.WithTag("mykey1", "myvalue1");
                upload.WithTag("mykey2", "myvalue2");
            }).ConfigureAwait(false);

            GetObjectResponse gResp = await AssertAsync(nameof(UploadMultipleTags)).ConfigureAwait(false);
            Assert.Equal(2, gResp.TagCount);
        }

        [Fact]
        public async Task UploadServerSideEncryption()
        {
            PutObjectResponse resp = await UploadTransferAsync(nameof(UploadServerSideEncryption) + "aes", upload => upload.WithEncryption()).ConfigureAwait(false);
            Assert.Equal(SseAlgorithm.Aes256, resp.SseAlgorithm);

            await AssertAsync(nameof(UploadServerSideEncryption) + "aes").ConfigureAwait(false);

            resp = await UploadTransferAsync(nameof(UploadServerSideEncryption) + "kms", upload => upload.WithEncryptionKms()).ConfigureAwait(false);
            Assert.Equal(SseAlgorithm.AwsKms, resp.SseAlgorithm);

            await AssertAsync(nameof(UploadServerSideEncryption) + "kms").ConfigureAwait(false);
        }

        [Fact]
        public async Task UploadServerSideEncryptionCustomerKey()
        {
            byte[] key = new byte[32];
            new Random(42).NextBytes(key);

            PutObjectResponse pResp = await UploadTransferAsync(nameof(UploadServerSideEncryptionCustomerKey), upload => upload.WithEncryptionCustomerKey(key)).ConfigureAwait(false);

            Assert.Equal(SseCustomerAlgorithm.Aes256, pResp.SseCustomerAlgorithm);

            await AssertTransferAsync(nameof(UploadServerSideEncryptionCustomerKey), download => download.WithEncryptionCustomerKey(key)).ConfigureAwait(false);
        }

        [Fact]
        public async Task UploadWebsiteRedirect()
        {
            await UploadTransferAsync(nameof(UploadWebsiteRedirect), upload => upload.WithWebsiteRedirectLocation("https://google.com")).ConfigureAwait(false);

            GetObjectResponse resp1 = await AssertAsync(nameof(UploadWebsiteRedirect)).ConfigureAwait(false);
            Assert.Equal("https://google.com", resp1.WebsiteRedirectLocation);
            Assert.Equal(200, resp1.StatusCode);
        }
    }
}