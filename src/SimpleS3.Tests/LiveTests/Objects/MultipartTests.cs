using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Responses.Errors;
using Genbox.SimpleS3.Core.Responses.Objects;
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
            string resourceName = nameof(AbortIncompleteUpload);

            CreateMultipartUploadResponse initResp = await ObjectClient.CreateMultipartUploadAsync(BucketName, resourceName).ConfigureAwait(false);

            Assert.Equal(BucketName, initResp.Bucket);
            Assert.Equal(resourceName, initResp.Key);
            Assert.NotNull(initResp.UploadId);

            AbortMultipartUploadResponse abortResp = await ObjectClient.AbortMultipartUploadAsync(BucketName, resourceName, initResp.UploadId).ConfigureAwait(false);

            Assert.True(abortResp.IsSuccess);
            Assert.Equal(204, abortResp.StatusCode);
        }

        [Fact]
        public async Task ManualMultiplePartUpload()
        {
            string resourceName = nameof(ManualMultiplePartUpload);

            byte[] key = { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[] keyMd5 = CryptoHelper.Md5Hash(key);

            CreateMultipartUploadResponse initResp = await ObjectClient.CreateMultipartUploadAsync(BucketName, resourceName, request =>
            {
                request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                request.SseCustomerKey = key;
                request.SseCustomerKeyMd5 = keyMd5;
            }).ConfigureAwait(false);

            Assert.True(initResp.IsSuccess);
            Assert.Equal(BucketName, initResp.Bucket);
            Assert.Equal(resourceName, initResp.Key);
            Assert.NotNull(initResp.UploadId);

            byte[] file = new byte[1024 * 1024 * 10];
            file[0] = (byte)'a';
            file[file.Length - 1] = (byte)'b';

            byte[][] parts = file.Chunk(file.Length / 2).Select(x => x.ToArray()).ToArray();

            UploadPartResponse uploadResp1 = await ObjectClient.UploadPartAsync(BucketName, resourceName, 1, initResp.UploadId, new MemoryStream(parts[0]), request =>
            {
                request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                request.SseCustomerKey = key;
                request.SseCustomerKeyMd5 = keyMd5;
            }).ConfigureAwait(false);

            Assert.Equal(SseCustomerAlgorithm.Aes256, uploadResp1.SseCustomerAlgorithm);
            Assert.Equal(keyMd5, uploadResp1.SseCustomerKeyMd5);

            Assert.True(uploadResp1.IsSuccess);
            Assert.NotNull(uploadResp1.ETag);

            UploadPartResponse uploadResp2 = await ObjectClient.UploadPartAsync(BucketName, resourceName, 2, initResp.UploadId, new MemoryStream(parts[1]), request =>
            {
                request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                request.SseCustomerKey = key;
                request.SseCustomerKeyMd5 = keyMd5;
            }).ConfigureAwait(false);

            Assert.True(uploadResp2.IsSuccess);
            Assert.NotNull(uploadResp2.ETag);

            CompleteMultipartUploadResponse completeResp = await ObjectClient.CompleteMultipartUploadAsync(BucketName, resourceName, initResp.UploadId, new[] { uploadResp1, uploadResp2 }).ConfigureAwait(false);

            Assert.True(completeResp.IsSuccess);
            Assert.NotNull(uploadResp2.ETag);

            GetObjectResponse getResp = await ObjectClient.GetObjectAsync(BucketName, resourceName, request =>
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
            string resourceName = nameof(ManualSinglePartUpload);

            CreateMultipartUploadResponse initResp = await ObjectClient.CreateMultipartUploadAsync(BucketName, resourceName, request =>
            {
                request.SseAlgorithm = SseAlgorithm.Aes256;
                request.StorageClass = StorageClass.StandardIa;
            }).ConfigureAwait(false);

            Assert.True(initResp.IsSuccess);
            Assert.Equal(BucketName, initResp.Bucket);
            Assert.Equal(resourceName, initResp.Key);
            Assert.NotNull(initResp.UploadId);

            byte[] file = new byte[1024 * 1024 * 5];
            file[0] = (byte)'a';

            UploadPartResponse uploadResp = await ObjectClient.UploadPartAsync(BucketName, resourceName, 1, initResp.UploadId, new MemoryStream(file)).ConfigureAwait(false);
            Assert.True(uploadResp.IsSuccess);
            Assert.NotNull(uploadResp.ETag);
            Assert.Equal(SseAlgorithm.Aes256, uploadResp.SseAlgorithm);
            Assert.Equal(StorageClass.StandardIa, uploadResp.StorageClass);

            CompleteMultipartUploadResponse completeResp = await ObjectClient.CompleteMultipartUploadAsync(BucketName, resourceName, initResp.UploadId, new[] { uploadResp }).ConfigureAwait(false);

            Assert.True(completeResp.IsSuccess);
            Assert.NotNull(completeResp.ETag);

            GetObjectResponse resp = await ObjectClient.GetObjectAsync(BucketName, resourceName).ConfigureAwait(false);
            Assert.True(resp.IsSuccess);
            Assert.Equal(file, await resp.Content.AsDataAsync().ConfigureAwait(false));
        }

        [Fact]
        public async Task MultipartFluid()
        {
            byte[] data = new byte[10 * 1024 * 1024]; //10 Mb

            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)(i % 255);

            MultipartUploadStatus resp = await Transfer.UploadData(BucketName, nameof(MultipartFluid), data)
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
            string resourceName = nameof(TooSmallUpload);

            CreateMultipartUploadResponse initResp = await ObjectClient.CreateMultipartUploadAsync(BucketName, resourceName).ConfigureAwait(false);

            Assert.Equal(BucketName, initResp.Bucket);
            Assert.Equal(resourceName, initResp.Key);
            Assert.NotNull(initResp.UploadId);

            //4 MB is below the 5 MB limit. See https://docs.aws.amazon.com/AmazonS3/latest/dev/qfacts.html
            //Note that if there only is 1 part, then it is technically the last part, and can be of any size. That's why this test has 2 parts.
            byte[] file = new byte[1024 * 1024 * 4];

            byte[][] chunks = file.Chunk(file.Length / 2).Select(x => x.ToArray()).ToArray();

            UploadPartResponse uploadResp1 = await ObjectClient.UploadPartAsync(BucketName, resourceName, 1, initResp.UploadId, new MemoryStream(chunks[0])).ConfigureAwait(false);

            Assert.True(uploadResp1.IsSuccess);
            Assert.NotNull(uploadResp1.ETag);

            UploadPartResponse uploadResp2 = await ObjectClient.UploadPartAsync(BucketName, resourceName, 2, initResp.UploadId, new MemoryStream(chunks[1])).ConfigureAwait(false);

            Assert.True(uploadResp2.IsSuccess);
            Assert.NotNull(uploadResp2.ETag);

            CompleteMultipartUploadResponse completeResp = await ObjectClient.CompleteMultipartUploadAsync(BucketName, resourceName, initResp.UploadId, new[] { uploadResp1, uploadResp2 }).ConfigureAwait(false);

            Assert.False(completeResp.IsSuccess);
            Assert.Equal(400, completeResp.StatusCode);
        }
    }
}