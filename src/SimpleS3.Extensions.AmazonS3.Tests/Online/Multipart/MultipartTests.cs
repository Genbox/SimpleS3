using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Errors;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;
#if COMMERCIAL
using System.Collections.Generic;
#endif

namespace Genbox.SimpleS3.Extensions.AmazonS3.Tests.Online.Multipart
{
    public class MultipartTests : AwsTestBase
    {
        public MultipartTests(ITestOutputHelper helper) : base(helper) { }

        [Theory]
        [InlineData(SseAlgorithm.Aes256)]
        [InlineData(SseAlgorithm.AwsKms)]
        public async Task MultipartWithEncryption(SseAlgorithm algorithm)
        {
            string objectKey = nameof(MultipartWithEncryption);

            CreateMultipartUploadResponse createResp = await MultipartClient.CreateMultipartUploadAsync(BucketName, objectKey, req => req.SseAlgorithm = algorithm).ConfigureAwait(false);
            Assert.Equal(algorithm, createResp.SseAlgorithm);

            if (algorithm == SseAlgorithm.AwsKms)
                Assert.NotNull(createResp.SseKmsKeyId);

            await using MemoryStream ms = new MemoryStream(new byte[1024 * 1024 * 5]);

            UploadPartResponse uploadResp = await MultipartClient.UploadPartAsync(BucketName, objectKey, 1, createResp.UploadId, ms).ConfigureAwait(false);
            Assert.Equal(algorithm, uploadResp.SseAlgorithm);

            if (algorithm == SseAlgorithm.AwsKms)
                Assert.NotNull(uploadResp.SseKmsKeyId);

            CompleteMultipartUploadResponse completeResp = await MultipartClient.CompleteMultipartUploadAsync(BucketName, objectKey, createResp.UploadId, new[] { uploadResp }).ConfigureAwait(false);
            Assert.Equal(algorithm, completeResp.SseAlgorithm);

            if (algorithm == SseAlgorithm.AwsKms)
                Assert.NotNull(completeResp.SseKmsKeyId);
        }

        [Fact]
        public async Task MultipartCustomerEncryption()
        {
            string objectKey = nameof(MultipartCustomerEncryption);

            byte[] key = { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[] keyMd5 = CryptoHelper.Md5Hash(key);

            CreateMultipartUploadResponse initResp = await MultipartClient.CreateMultipartUploadAsync(BucketName, objectKey, req =>
            {
                req.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                req.SseCustomerKey = key;
                req.SseCustomerKeyMd5 = keyMd5;
            }).ConfigureAwait(false);

            Assert.True(initResp.IsSuccess);

            await using MemoryStream ms = new MemoryStream(new byte[1024 * 1024 * 5]);

            UploadPartResponse uploadResp1 = await MultipartClient.UploadPartAsync(BucketName, objectKey, 1, initResp.UploadId, ms, req =>
            {
                req.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
                req.SseCustomerKey = key;
                req.SseCustomerKeyMd5 = keyMd5;
            }).ConfigureAwait(false);

            Assert.Equal(SseCustomerAlgorithm.Aes256, uploadResp1.SseCustomerAlgorithm);
            Assert.Equal(keyMd5, uploadResp1.SseCustomerKeyMd5);

            CompleteMultipartUploadResponse completeResp = await MultipartClient.CompleteMultipartUploadAsync(BucketName, objectKey, initResp.UploadId, new[] { uploadResp1 }).ConfigureAwait(false);
            Assert.True(completeResp.IsSuccess);
            Assert.Equal(SseCustomerAlgorithm.Aes256, completeResp.SseCustomerAlgorithm);
        }

        [Fact]
        public async Task MultipartLockMode()
        {
            string objectKey = nameof(MultipartLockMode);

            CreateMultipartUploadResponse initResp = await MultipartClient.CreateMultipartUploadAsync(BucketName, objectKey, req =>
            {
                req.LockMode = LockMode.Governance;
                req.LockRetainUntil = DateTimeOffset.UtcNow.AddMinutes(5);
            }).ConfigureAwait(false);

            Assert.True(initResp.IsSuccess);

            byte[] file = new byte[1024 * 1024 * 5];
            await using MemoryStream ms = new MemoryStream(file);

            UploadPartResponse uploadResp = await MultipartClient.UploadPartAsync(BucketName, objectKey, 1, initResp.UploadId, ms, req => req.ContentMd5 = CryptoHelper.Md5Hash(file)).ConfigureAwait(false);
            Assert.True(uploadResp.IsSuccess);

            CompleteMultipartUploadResponse completeResp = await MultipartClient.CompleteMultipartUploadAsync(BucketName, objectKey, initResp.UploadId, new[] { uploadResp }).ConfigureAwait(false);
            Assert.True(completeResp.IsSuccess);
        }

        [Fact(Skip = "Require a setup of another AWS account with 'Requester pays' setup")]
        public async Task MultipartRequestPayer()
        {
            string objectKey = nameof(MultipartRequestPayer);

            CreateMultipartUploadResponse initResp = await MultipartClient.CreateMultipartUploadAsync(BucketName, objectKey, req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(initResp.RequestCharged);

            await using MemoryStream ms = new MemoryStream(new byte[1024 * 1024 * 5]);

            UploadPartResponse uploadResp = await MultipartClient.UploadPartAsync(BucketName, objectKey, 1, initResp.UploadId, ms, req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(uploadResp.RequestCharged);

            ListPartsResponse listResp = await MultipartClient.ListPartsAsync(BucketName, objectKey, initResp.UploadId, req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(listResp.RequestCharged);

            CompleteMultipartUploadResponse completeResp = await MultipartClient.CompleteMultipartUploadAsync(BucketName, objectKey, initResp.UploadId, new[] { uploadResp }, req => req.RequestPayer = Payer.Requester).ConfigureAwait(false);
            Assert.True(completeResp.RequestCharged);
        }

        [Fact]
        public async Task MultipartSinglePart()
        {
            string objectKey = nameof(MultipartSinglePart);

            CreateMultipartUploadResponse createResp = await MultipartClient.CreateMultipartUploadAsync(BucketName, objectKey).ConfigureAwait(false);
            Assert.True(createResp.IsSuccess);
            Assert.Equal(BucketName, createResp.BucketName);
            Assert.Equal(objectKey, createResp.ObjectKey);
            Assert.NotNull(createResp.UploadId);

            //Test lifecycle expiration
            Assert.Equal(DateTime.UtcNow.AddDays(2).Date, createResp.AbortsOn!.Value.UtcDateTime.Date);
            Assert.Equal("ExpireAll", createResp.AbortRuleId);

            byte[] file = new byte[1024 * 1024 * 5];
            file[0] = (byte)'a';

            await using MemoryStream ms = new MemoryStream(file);

            UploadPartResponse uploadResp = await MultipartClient.UploadPartAsync(BucketName, objectKey, 1, createResp.UploadId, ms).ConfigureAwait(false);
            Assert.True(uploadResp.IsSuccess);
            Assert.Equal("\"10f74ef02085310ccd1f87150b83e537\"", uploadResp.ETag);

            CompleteMultipartUploadResponse completeResp = await MultipartClient.CompleteMultipartUploadAsync(BucketName, objectKey, createResp.UploadId, new[] { uploadResp }).ConfigureAwait(false);
            Assert.True(completeResp.IsSuccess);
            Assert.Equal("\"bd74e21dfa8678d127240f76e518e9c2-1\"", completeResp.ETag);
            Assert.NotNull(completeResp.VersionId);

            //Test lifecycle expiration
            Assert.Equal(DateTime.UtcNow.AddDays(2).Date, completeResp.LifeCycleExpiresOn!.Value.UtcDateTime.Date);
            Assert.Equal("ExpireAll", completeResp.LifeCycleRuleId);
        }

        [Fact]
        public async Task MultipartTooSmall()
        {
            string objectKey = nameof(MultipartTooSmall);

            CreateMultipartUploadResponse initResp = await MultipartClient.CreateMultipartUploadAsync(BucketName, objectKey).ConfigureAwait(false);

            Assert.Equal(BucketName, initResp.BucketName);
            Assert.Equal(objectKey, initResp.ObjectKey);
            Assert.NotNull(initResp.UploadId);

            //4 MB is below the 5 MB limit. See https://docs.aws.amazon.com/AmazonS3/latest/dev/qfacts.html
            //Note that if there only is 1 part, then it is technically the last part, and can be of any size. That's why this test has 2 parts.
            byte[] file = new byte[1024 * 1024 * 4];
            byte[][] parts = file.Chunk(file.Length / 2).Select(x => x.ToArray()).ToArray();

            await using MemoryStream ms1 = new MemoryStream(parts[0]);
            UploadPartResponse uploadResp1 = await MultipartClient.UploadPartAsync(BucketName, objectKey, 1, initResp.UploadId, ms1).ConfigureAwait(false);

            Assert.True(uploadResp1.IsSuccess);
            Assert.NotNull(uploadResp1.ETag);

            await using MemoryStream ms2 = new MemoryStream(parts[0]);
            UploadPartResponse uploadResp2 = await MultipartClient.UploadPartAsync(BucketName, objectKey, 2, initResp.UploadId, ms2).ConfigureAwait(false);

            Assert.True(uploadResp2.IsSuccess);
            Assert.NotNull(uploadResp2.ETag);

            CompleteMultipartUploadResponse completeResp = await MultipartClient.CompleteMultipartUploadAsync(BucketName, objectKey, initResp.UploadId, new[] { uploadResp1, uploadResp2 }).ConfigureAwait(false);

            Assert.False(completeResp.IsSuccess);
            Assert.Equal(400, completeResp.StatusCode);
        }

        [Fact]
        public async Task MultipartUpload()
        {
            string objectKey = nameof(MultipartUpload);

            CreateMultipartUploadResponse initResp = await MultipartClient.CreateMultipartUploadAsync(BucketName, objectKey).ConfigureAwait(false);

            Assert.True(initResp.IsSuccess);
            Assert.Equal(BucketName, initResp.BucketName);
            Assert.Equal(objectKey, initResp.ObjectKey);
            Assert.NotNull(initResp.UploadId);

            byte[] file = new byte[1024 * 1024 * 10];
            file[0] = (byte)'a';
            file[^1] = (byte)'b';

            byte[][] parts = file.Chunk(file.Length / 2).Select(x => x.ToArray()).ToArray();

            await using MemoryStream ms1 = new MemoryStream(parts[0]);
            UploadPartResponse uploadResp1 = await MultipartClient.UploadPartAsync(BucketName, objectKey, 1, initResp.UploadId, ms1).ConfigureAwait(false);

            Assert.True(uploadResp1.IsSuccess);
            Assert.NotNull(uploadResp1.ETag);

            await using MemoryStream ms2 = new MemoryStream(parts[0]);
            UploadPartResponse uploadResp2 = await MultipartClient.UploadPartAsync(BucketName, objectKey, 2, initResp.UploadId, ms2).ConfigureAwait(false);

            Assert.True(uploadResp2.IsSuccess);
            Assert.NotNull(uploadResp2.ETag);

            CompleteMultipartUploadResponse completeResp = await MultipartClient.CompleteMultipartUploadAsync(BucketName, objectKey, initResp.UploadId, new[] { uploadResp1, uploadResp2 }).ConfigureAwait(false);
            Assert.True(completeResp.IsSuccess);
            Assert.NotNull(uploadResp2.ETag);

            //Provoke an 'InvalidArgument' error. Parts start from index 1
            GetObjectResponse gResp1 = await ObjectClient.GetObjectAsync(BucketName, nameof(MultipartUpload), req => req.PartNumber = 0).ConfigureAwait(false);
            Assert.False(gResp1.IsSuccess);
            Assert.IsType<InvalidArgumentError>(gResp1.Error);

            GetObjectResponse gResp2 = await ObjectClient.GetObjectAsync(BucketName, nameof(MultipartUpload), req => req.PartNumber = 1).ConfigureAwait(false);
            Assert.True(gResp2.IsSuccess);

            byte[] contentData = await gResp2.Content!.AsDataAsync().ConfigureAwait(false);
            Assert.Equal(parts[0].Length, contentData.Length);
            Assert.Equal(parts[0], contentData);
        }

#if COMMERCIAL
        [Fact]
        public async Task MultipartViaClient()
        {
            byte[] data = new byte[20 * 1024 * 1024]; //20 Mb

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)(i % 255);
            }

            await using (MemoryStream ms = new MemoryStream(data))
            {
                CompleteMultipartUploadResponse resp = await MultipartTransfer.MultipartUploadAsync(BucketName, nameof(MultipartViaClient), ms, 5 * 1024 * 1024).ConfigureAwait(false);

                Assert.True(resp.IsSuccess);
            }

            GetObjectResponse getResp = await ObjectClient.GetObjectAsync(BucketName, nameof(MultipartViaClient)).ConfigureAwait(false);
            Assert.True(getResp.IsSuccess);

            await using (MemoryStream ms = new MemoryStream())
            {
                await getResp.Content.CopyToAsync(ms).ConfigureAwait(false);
                Assert.Equal(data, ms.ToArray());
            }

            //Try multipart downloading it
            await using (MemoryStream ms = new MemoryStream())
            {
                IAsyncEnumerable<GetObjectResponse> responses = MultipartTransfer.MultipartDownloadAsync(BucketName, nameof(MultipartViaClient), ms);

                await foreach (GetObjectResponse resp in responses)
                {
                    Assert.True(resp.IsSuccess);
                }

                Assert.Equal(data, ms.ToArray());
            }

            HeadObjectResponse headResp = await ObjectClient.HeadObjectAsync(BucketName, nameof(MultipartViaClient), req => req.PartNumber = 1).ConfigureAwait(false);
            Assert.Equal(4, headResp.NumberOfParts);
        }

        [Fact]
        public async Task MultipartViaExtensions()
        {
            byte[] data = new byte[100 * 1024 * 1024]; //100 Mb

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)'A';
            }

            int count = 0;
            await using (MemoryStream ms = new MemoryStream(data))
            {
                CompleteMultipartUploadResponse uploadResp = await MultipartTransfer.MultipartUploadAsync(BucketName, nameof(MultipartViaExtensions), ms, 10 * 1024 * 1024, 2, null, response =>
                {
                    Assert.True(response.IsSuccess);
                    count++;
                });

                Assert.NotNull(uploadResp);
                Assert.True(uploadResp!.IsSuccess);
            }

            Assert.Equal(10, count);
        }
#endif
    }
}