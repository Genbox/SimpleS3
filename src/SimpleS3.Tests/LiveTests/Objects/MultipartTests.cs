using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Network.Responses.Errors;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Tests.Code.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class MultipartTests : LiveTestBase
    {
        public MultipartTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task AbortIncompleteUpload()
        {
            string objectKey = nameof(AbortIncompleteUpload);

            CreateMultipartUploadResponse initResp = await ObjectClient.CreateMultipartUploadAsync(BucketName, objectKey).ConfigureAwait(false);

            Assert.Equal(BucketName, initResp.Bucket);
            Assert.Equal(objectKey, initResp.ObjectKey);
            Assert.NotNull(initResp.UploadId);

            AbortMultipartUploadResponse abortResp = await ObjectClient.AbortMultipartUploadAsync(BucketName, objectKey, initResp.UploadId).ConfigureAwait(false);

            Assert.True(abortResp.IsSuccess);
            Assert.Equal(204, abortResp.StatusCode);
        }

        [Fact]
        public async Task ManualMultiplePartUpload()
        {
            string objectKey = nameof(ManualMultiplePartUpload);

            byte[] key = { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[] keyMd5 = CryptoHelper.Md5Hash(key);

            CreateMultipartUploadResponse initResp = await ObjectClient.CreateMultipartUploadAsync(BucketName, objectKey, request =>
            {
                request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                request.SseCustomerKey = key;
                request.SseCustomerKeyMd5 = keyMd5;
            }).ConfigureAwait(false);

            Assert.True(initResp.IsSuccess);
            Assert.Equal(BucketName, initResp.Bucket);
            Assert.Equal(objectKey, initResp.ObjectKey);
            Assert.NotNull(initResp.UploadId);

            byte[] file = new byte[1024 * 1024 * 10];
            file[0] = (byte)'a';
            file[file.Length - 1] = (byte)'b';

            byte[][] parts = file.Chunk(file.Length / 2).Select(x => x.ToArray()).ToArray();

            UploadPartResponse uploadResp1 = await ObjectClient.UploadPartAsync(BucketName, objectKey, 1, initResp.UploadId, new MemoryStream(parts[0]), request =>
            {
                request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                request.SseCustomerKey = key;
                request.SseCustomerKeyMd5 = keyMd5;
            }).ConfigureAwait(false);

            Assert.Equal(SseCustomerAlgorithm.Aes256, uploadResp1.SseCustomerAlgorithm);
            Assert.Equal(keyMd5, uploadResp1.SseCustomerKeyMd5);

            Assert.True(uploadResp1.IsSuccess);
            Assert.NotNull(uploadResp1.ETag);

            UploadPartResponse uploadResp2 = await ObjectClient.UploadPartAsync(BucketName, objectKey, 2, initResp.UploadId, new MemoryStream(parts[1]), request =>
            {
                request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                request.SseCustomerKey = key;
                request.SseCustomerKeyMd5 = keyMd5;
            }).ConfigureAwait(false);

            Assert.True(uploadResp2.IsSuccess);
            Assert.NotNull(uploadResp2.ETag);

            CompleteMultipartUploadResponse completeResp = await ObjectClient.CompleteMultipartUploadAsync(BucketName, objectKey, initResp.UploadId, new[] { uploadResp1, uploadResp2 }).ConfigureAwait(false);

            Assert.True(completeResp.IsSuccess);
            Assert.NotNull(uploadResp2.ETag);

            GetObjectResponse getResp = await ObjectClient.GetObjectAsync(BucketName, objectKey, request =>
            {
                request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                request.SseCustomerKey = key;
                request.SseCustomerKeyMd5 = keyMd5;
            }).ConfigureAwait(false);

            //Provoke an 'InvalidArgument' error
            GetObjectResponse gResp1 = await ObjectClient.GetObjectAsync(BucketName, nameof(ManualMultiplePartUpload), request =>
            {
                request.PartNumber = 0;
                request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                request.SseCustomerKey = key;
                request.SseCustomerKeyMd5 = keyMd5;
            }).ConfigureAwait(false);
            Assert.False(gResp1.IsSuccess);
            Assert.IsType<InvalidArgumentError>(gResp1.Error);

            GetObjectResponse gResp2 = await ObjectClient.GetObjectAsync(BucketName, nameof(ManualMultiplePartUpload), request =>
            {
                request.PartNumber = 1;
                request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                request.SseCustomerKey = key;
                request.SseCustomerKeyMd5 = keyMd5;
            }).ConfigureAwait(false);

            Assert.True(gResp2.IsSuccess);
            Assert.Equal(file.Length / 2, gResp2.Content.AsStream().Length);
            Assert.Equal(file, await getResp.Content.AsDataAsync().ConfigureAwait(false));
        }

        [Fact]
        public async Task ManualSinglePartUpload()
        {
            string objectKey = nameof(ManualSinglePartUpload);

            CreateMultipartUploadResponse initResp = await ObjectClient.CreateMultipartUploadAsync(BucketName, objectKey, request =>
            {
                request.SseAlgorithm = SseAlgorithm.Aes256;
                request.StorageClass = StorageClass.StandardIa;
            }).ConfigureAwait(false);

            Assert.True(initResp.IsSuccess);
            Assert.Equal(BucketName, initResp.Bucket);
            Assert.Equal(objectKey, initResp.ObjectKey);
            Assert.NotNull(initResp.UploadId);

            byte[] file = new byte[1024 * 1024 * 5];
            file[0] = (byte)'a';

            UploadPartResponse uploadResp = await ObjectClient.UploadPartAsync(BucketName, objectKey, 1, initResp.UploadId, new MemoryStream(file)).ConfigureAwait(false);
            Assert.True(uploadResp.IsSuccess);
            Assert.NotNull(uploadResp.ETag);
            Assert.Equal(SseAlgorithm.Aes256, uploadResp.SseAlgorithm);
            Assert.Equal(StorageClass.StandardIa, uploadResp.StorageClass);

            CompleteMultipartUploadResponse completeResp = await ObjectClient.CompleteMultipartUploadAsync(BucketName, objectKey, initResp.UploadId, new[] { uploadResp }).ConfigureAwait(false);

            Assert.True(completeResp.IsSuccess);
            Assert.NotNull(completeResp.ETag);

            GetObjectResponse resp = await ObjectClient.GetObjectAsync(BucketName, objectKey).ConfigureAwait(false);
            Assert.True(resp.IsSuccess);
            Assert.Equal(file, await resp.Content.AsDataAsync().ConfigureAwait(false));
        }

        [Fact]
        public async Task MultipartFluent()
        {
            byte[] data = new byte[10 * 1024 * 1024]; //10 Mb

            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)(i % 255);

            MultipartUploadStatus resp = await Transfer.UploadData(BucketName, nameof(MultipartFluent), data)
                .ExecuteMultipartAsync()
                .ConfigureAwait(false);

            Assert.Equal(MultipartUploadStatus.Ok, resp);

            GetObjectResponse getResp = await ObjectClient.GetObjectAsync(BucketName, nameof(MultipartViaClient)).ConfigureAwait(false);
            Assert.True(getResp.IsSuccess);

            using (MemoryStream ms = new MemoryStream())
            {
                await getResp.Content.CopyToAsync(ms).ConfigureAwait(false);
                Assert.Equal(data, ms.ToArray());
            }

            //TODO: Need Download Transfer support first
            //Try multipart downloading it
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    await ObjectClient.MultipartDownloadAsync(BucketName, nameof(MultipartViaClient), ms).ConfigureAwait(false);
            //    Assert.Equal(data, ms.ToArray());
            //}

            HeadObjectResponse headResp = await ObjectClient.HeadObjectAsync(BucketName, nameof(MultipartViaClient), req => req.PartNumber = 1).ConfigureAwait(false);
            Assert.Equal(2, headResp.NumberOfParts);
        }

        [Fact]
        public async Task MultipartViaClient()
        {
            byte[] data = new byte[10 * 1024 * 1024]; //10 Mb

            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)(i % 255);

            using (MemoryStream ms = new MemoryStream(data))
            {
                MultipartUploadStatus resp = await ObjectClient.MultipartUploadAsync(BucketName, nameof(MultipartViaClient), ms, 5 * 1024 * 1024).ConfigureAwait(false);

                Assert.Equal(MultipartUploadStatus.Ok, resp);
            }

            GetObjectResponse getResp = await ObjectClient.GetObjectAsync(BucketName, nameof(MultipartViaClient)).ConfigureAwait(false);
            Assert.True(getResp.IsSuccess);

            using (MemoryStream ms = new MemoryStream())
            {
                await getResp.Content.CopyToAsync(ms).ConfigureAwait(false);
                Assert.Equal(data, ms.ToArray());
            }

            //Try multipart downloading it
            using (MemoryStream ms = new MemoryStream())
            {
                await ObjectClient.MultipartDownloadAsync(BucketName, nameof(MultipartViaClient), ms).ConfigureAwait(false);
                Assert.Equal(data, ms.ToArray());
            }

            HeadObjectResponse headResp = await ObjectClient.HeadObjectAsync(BucketName, nameof(MultipartViaClient), req => req.PartNumber = 1).ConfigureAwait(false);
            Assert.Equal(2, headResp.NumberOfParts);
        }

        [Fact]
        public async Task TooSmallUpload()
        {
            string objectKey = nameof(TooSmallUpload);

            CreateMultipartUploadResponse initResp = await ObjectClient.CreateMultipartUploadAsync(BucketName, objectKey).ConfigureAwait(false);

            Assert.Equal(BucketName, initResp.Bucket);
            Assert.Equal(objectKey, initResp.ObjectKey);
            Assert.NotNull(initResp.UploadId);

            //4 MB is below the 5 MB limit. See https://docs.aws.amazon.com/AmazonS3/latest/dev/qfacts.html
            //Note that if there only is 1 part, then it is technically the last part, and can be of any size. That's why this test has 2 parts.
            byte[] file = new byte[1024 * 1024 * 4];

            byte[][] chunks = file.Chunk(file.Length / 2).Select(x => x.ToArray()).ToArray();

            UploadPartResponse uploadResp1 = await ObjectClient.UploadPartAsync(BucketName, objectKey, 1, initResp.UploadId, new MemoryStream(chunks[0])).ConfigureAwait(false);

            Assert.True(uploadResp1.IsSuccess);
            Assert.NotNull(uploadResp1.ETag);

            UploadPartResponse uploadResp2 = await ObjectClient.UploadPartAsync(BucketName, objectKey, 2, initResp.UploadId, new MemoryStream(chunks[1])).ConfigureAwait(false);

            Assert.True(uploadResp2.IsSuccess);
            Assert.NotNull(uploadResp2.ETag);

            CompleteMultipartUploadResponse completeResp = await ObjectClient.CompleteMultipartUploadAsync(BucketName, objectKey, initResp.UploadId, new[] { uploadResp1, uploadResp2 }).ConfigureAwait(false);

            Assert.False(completeResp.IsSuccess);
            Assert.Equal(400, completeResp.StatusCode);
        }

        [Fact]
        public async Task ListParts()
        {
            await CreateTempBucketAsync(async bucket =>
            {
                CreateMultipartUploadResponse initResp = await ObjectClient.CreateMultipartUploadAsync(bucket, nameof(ListParts)).ConfigureAwait(false);

                ListPartsResponse listResp1 = await ObjectClient.ListPartsAsync(bucket, nameof(ListParts), initResp.UploadId).ConfigureAwait(false);

                Assert.Equal(bucket, listResp1.BucketName);
                Assert.Equal("ListParts", listResp1.ObjectKey);
                Assert.Equal(initResp.UploadId, listResp1.UploadId);
                Assert.Equal(StorageClass.Standard, listResp1.StorageClass);
                Assert.Equal(0, listResp1.PartNumberMarker);
                Assert.Equal(0, listResp1.NextPartNumberMarker);
                Assert.Equal(1000, listResp1.MaxParts);
                Assert.False(listResp1.IsTruncated);
                Assert.Equal(TestConstants.TestUsername, listResp1.Owner.Name);

                Assert.Empty(listResp1.Parts);

                UploadPartResponse uploadResp;

                byte[] file = new byte[5 * 1024];

                using (MemoryStream ms = new MemoryStream(file))
                    uploadResp = await ObjectClient.UploadPartAsync(bucket, nameof(ListParts), 1, initResp.UploadId, ms).ConfigureAwait(false);

                ListPartsResponse listResp2 = await ObjectClient.ListPartsAsync(bucket, nameof(ListParts), initResp.UploadId).ConfigureAwait(false);

                Assert.Equal(bucket, listResp2.BucketName);
                Assert.Equal("ListParts", listResp2.ObjectKey);
                Assert.Equal(initResp.UploadId, listResp2.UploadId);
                Assert.Equal(StorageClass.Standard, listResp2.StorageClass);
                Assert.Equal(0, listResp2.PartNumberMarker);
                Assert.Equal(1, listResp2.NextPartNumberMarker);
                Assert.Equal(1000, listResp2.MaxParts);
                Assert.False(listResp2.IsTruncated);
                Assert.Equal(TestConstants.TestUsername, listResp2.Owner.Name);

                S3Part part = Assert.Single(listResp2.Parts);

                Assert.Equal(1, part.PartNumber);
                Assert.Equal(DateTime.UtcNow, part.LastModified.DateTime, TimeSpan.FromSeconds(5));
                Assert.Equal("\"32ca18808933aa12e979375d07048a11\"", part.ETag);
                Assert.Equal(file.Length, part.Size);

                await ObjectClient.CompleteMultipartUploadAsync(bucket, nameof(ListParts), initResp.UploadId, new[] { uploadResp }).ConfigureAwait(false);

                ListPartsResponse listResp3 = await ObjectClient.ListPartsAsync(bucket, nameof(ListParts), initResp.UploadId).ConfigureAwait(false);
                Assert.False(listResp3.IsSuccess);
                Assert.Equal(404, listResp3.StatusCode);
            }).ConfigureAwait(false);
        }
    }
}