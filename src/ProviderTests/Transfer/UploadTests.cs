using System;
using System.Threading.Tasks;
using Genbox.HttpBuilders.Enums;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.TestBase;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Transfer
{
    public class UploadTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.All, LockMode.Compliance, LockMode.Governance)]
        public async Task UploadWithLock(LockMode lockMode, IProfile profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            DateTimeOffset lockRetainUntil = DateTimeOffset.UtcNow.AddMinutes(1);

            //We add a unique guid to prevent contamination across runs
            string objectKey = $"{nameof(UploadWithLock)}-{lockMode}-{Guid.NewGuid()}";

            await client.CreateUpload(bucketName, objectKey).WithLock(lockMode, lockRetainUntil).UploadAsync(null);

            GetObjectResponse resp = await client.GetObjectAsync(bucketName, objectKey).ConfigureAwait(false);
            Assert.Equal(lockMode, resp.LockMode);
            Assert.Equal(lockRetainUntil.DateTime, resp.LockRetainUntil!.Value.DateTime, TimeSpan.FromSeconds(1));
        }

        [Theory]
        [MultipleProviders(S3Provider.All, ObjectCannedAcl.AuthenticatedRead, ObjectCannedAcl.AwsExecRead, ObjectCannedAcl.BucketOwnerFullControl, ObjectCannedAcl.BucketOwnerRead, ObjectCannedAcl.Private, ObjectCannedAcl.PublicRead, ObjectCannedAcl.PublicReadWrite)]
        public Task UploadWithCannedAcl(ObjectCannedAcl acl, IProfile profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            return client.CreateUpload(bucketName, $"{nameof(UploadWithCannedAcl)}-{acl}").WithAccessControl(acl).UploadAsync(null);
        }

        [Theory]
        [MultipleProviders(S3Provider.All, StorageClass.Standard, StorageClass.DeepArchive, StorageClass.Glacier, StorageClass.IntelligentTiering, StorageClass.OneZoneIa, StorageClass.ReducedRedundancy, StorageClass.StandardIa)]
        public async Task UploadStorageClass(StorageClass storageClass, bool canDownload, IProfile profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            PutObjectResponse pResp = await client.CreateUpload(bucketName, nameof(UploadStorageClass) + "-" + storageClass).WithStorageClass(storageClass).UploadAsync(null);
            Assert.Equal(storageClass, pResp.StorageClass);

            if (storageClass is StorageClass.Glacier or StorageClass.DeepArchive)
                return;

            GetObjectResponse gResp = await client.GetObjectAsync(bucketName, nameof(UploadStorageClass) + "-" + storageClass).ConfigureAwait(false);
            Assert.Equal(storageClass, gResp.StorageClass);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task UploadCacheControl(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            await client.CreateUpload(bucketName, nameof(UploadCacheControl)).WithCacheControl(CacheControlType.MaxAge, 100).UploadAsync(null);

            GetObjectResponse gResp = await client.GetObjectAsync(bucketName, nameof(UploadCacheControl)).ConfigureAwait(false);
            Assert.Equal("max-age=100", gResp.CacheControl);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task UploadContentProperties(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            IUpload upload = client.CreateUpload(bucketName, nameof(UploadContentProperties));

            upload.WithContentDisposition(ContentDispositionType.Attachment, "filename.jpg");
            upload.WithContentEncoding(ContentEncodingType.Identity);
            upload.WithContentType("text/html", "utf-8");

            await upload.UploadAsync(null);

            GetObjectResponse resp = await client.GetObjectAsync(bucketName, nameof(UploadContentProperties)).ConfigureAwait(false);
            Assert.Equal(4, resp.ContentLength);
            Assert.Equal("attachment; filename*=\"filename.jpg\"", resp.ContentDisposition);
            Assert.Equal("text/html; charset=utf-8", resp.ContentType);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task UploadEtag(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            PutObjectResponse putResp = await client.CreateUpload(bucketName, nameof(UploadEtag)).UploadAsync(null);

            await client.CreateDownload(bucketName, nameof(UploadEtag)).WithEtagConditional(putResp.ETag).DownloadAsync();

            GetObjectResponse getResp = await client.CreateDownload(bucketName, nameof(UploadEtag)).WithEtagConditional("not the tag you are looking for").DownloadAsync();
            Assert.Equal(412, getResp.StatusCode);

            await client.CreateDownload(bucketName, nameof(UploadEtag)).WithEtagConditional(null, "not the tag you are looking for").DownloadAsync();

            GetObjectResponse getResp2 = await client.CreateDownload(bucketName, nameof(UploadEtag)).WithEtagConditional(null, putResp.ETag).DownloadAsync();
            Assert.Equal(304, getResp2.StatusCode);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task UploadMetadata(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            IUpload upload = client.CreateUpload(bucketName, nameof(UploadMetadata));

            for (int i = 0; i < 10; i++)
            {
                upload.WithMetadata("mykey" + i, "myvalue" + i);
            }

            await upload.UploadAsync(null);

            GetObjectResponse gResp = await client.GetObjectAsync(bucketName, nameof(UploadMetadata)).ConfigureAwait(false);
            Assert.Equal(10, gResp.Metadata!.Count);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public Task UploadMultiplePermissions(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            ObjectAclBuilder acl = new ObjectAclBuilder();
            acl.AddEmail(TestConstants.TestEmail, ObjectPermissions.Read | ObjectPermissions.ReadAcl | ObjectPermissions.WriteAcl | ObjectPermissions.FullControl);
            acl.AddUserId(TestConstants.TestUserId, ObjectPermissions.Read | ObjectPermissions.ReadAcl | ObjectPermissions.WriteAcl | ObjectPermissions.FullControl);

            return client.CreateUpload(bucketName, nameof(UploadMultiplePermissions)).WithAccessControl(acl).UploadAsync(null);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task UploadMultipleTags(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            await client.CreateUpload(bucketName, nameof(UploadMultipleTags))
                        .WithTag("mykey1", "myvalue1")
                        .WithTag("mykey2", "myvalue2")
                        .UploadAsync(null);

            GetObjectResponse gResp = await client.GetObjectAsync(bucketName, nameof(UploadMultipleTags)).ConfigureAwait(false);
            Assert.Equal(2, gResp.TagCount);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task UploadServerSideEncryption(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            PutObjectResponse resp = await client.CreateUpload(bucketName, nameof(UploadServerSideEncryption) + "aes")
                                                 .WithEncryption()
                                                 .UploadAsync(null);

            Assert.Equal(SseAlgorithm.Aes256, resp.SseAlgorithm);

            await client.GetObjectAsync(bucketName, nameof(UploadServerSideEncryption) + "aes").ConfigureAwait(false);

            resp = await client.CreateUpload(bucketName, nameof(UploadServerSideEncryption) + "kms")
                               .WithEncryptionKms()
                               .UploadAsync(null);

            Assert.Equal(SseAlgorithm.AwsKms, resp.SseAlgorithm);

            await client.GetObjectAsync(bucketName, nameof(UploadServerSideEncryption) + "kms").ConfigureAwait(false);
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task UploadServerSideEncryptionCustomerKey(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            byte[] key = new byte[32];
            new Random(42).NextBytes(key);

            PutObjectResponse pResp = await client.CreateUpload(bucketName, nameof(UploadServerSideEncryptionCustomerKey))
                                                  .WithEncryptionCustomerKey(key)
                                                  .UploadAsync(null);

            Assert.Equal(SseCustomerAlgorithm.Aes256, pResp.SseCustomerAlgorithm);

            await client.CreateDownload(bucketName, nameof(UploadServerSideEncryptionCustomerKey)).WithEncryptionCustomerKey(key).DownloadAsync();
        }

        [Theory]
        [MultipleProviders(S3Provider.All)]
        public async Task UploadWebsiteRedirect(S3Provider _, IProfile  profile, ISimpleClient client)
        {
            string bucketName = GetTestBucket(profile);
            await client.CreateUpload(bucketName, nameof(UploadWebsiteRedirect))
                        .WithWebsiteRedirectLocation("https://google.com")
                        .UploadAsync(null);

            GetObjectResponse resp1 = await client.GetObjectAsync(bucketName, nameof(UploadWebsiteRedirect)).ConfigureAwait(false);
            Assert.Equal("https://google.com", resp1.WebsiteRedirectLocation);
            Assert.Equal(200, resp1.StatusCode);
        }
    }
}