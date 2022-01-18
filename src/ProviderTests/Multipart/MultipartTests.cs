using Genbox.ProviderTests.Misc;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common.Extensions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Errors;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Multipart;

public class MultipartTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3, SseAlgorithm.Aes256, SseAlgorithm.AwsKms)]
    [MultipleProviders(S3Provider.GoogleCloudStorage, SseAlgorithm.Aes256)]
    public async Task MultipartWithEncryption(S3Provider provider, string bucket, ISimpleClient client, SseAlgorithm algorithm)
    {
        string objectKey = nameof(MultipartWithEncryption);

        CreateMultipartUploadResponse createResp = await client.CreateMultipartUploadAsync(bucket, objectKey, req => req.SseAlgorithm = algorithm).ConfigureAwait(false);
        Assert.Equal(200, createResp.StatusCode);

        if (provider == S3Provider.AmazonS3)
            Assert.Equal(algorithm, createResp.SseAlgorithm);

        if (algorithm == SseAlgorithm.AwsKms)
            Assert.NotNull(createResp.SseKmsKeyId);

        await using MemoryStream ms = new MemoryStream(new byte[1024 * 1024 * 5]);

        UploadPartResponse uploadResp = await client.UploadPartAsync(bucket, objectKey, 1, createResp.UploadId, ms).ConfigureAwait(false);
        Assert.Equal(200, uploadResp.StatusCode);

        if (provider == S3Provider.AmazonS3)
            Assert.Equal(algorithm, uploadResp.SseAlgorithm);

        if (algorithm == SseAlgorithm.AwsKms)
            Assert.NotNull(uploadResp.SseKmsKeyId);

        CompleteMultipartUploadResponse completeResp = await client.CompleteMultipartUploadAsync(bucket, objectKey, createResp.UploadId, new[] { uploadResp }).ConfigureAwait(false);
        Assert.Equal(200, completeResp.StatusCode);

        if (provider == S3Provider.AmazonS3)
            Assert.Equal(algorithm, completeResp.SseAlgorithm);

        if (algorithm == SseAlgorithm.AwsKms)
            Assert.NotNull(completeResp.SseKmsKeyId);
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task MultipartCustomerEncryption(S3Provider _, string bucket, ISimpleClient client)
    {
        string objectKey = nameof(MultipartCustomerEncryption);

        byte[] key = { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8 };
        byte[] keyMd5 = CryptoHelper.Md5Hash(key);

        CreateMultipartUploadResponse initResp = await client.CreateMultipartUploadAsync(bucket, objectKey, r =>
        {
            r.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
            r.SseCustomerKey = key;
            r.SseCustomerKeyMd5 = keyMd5;
        }).ConfigureAwait(false);
        Assert.Equal(200, initResp.StatusCode);

        await using MemoryStream ms = new MemoryStream(new byte[1024 * 1024 * 5]);

        UploadPartResponse uploadResp1 = await client.UploadPartAsync(bucket, objectKey, 1, initResp.UploadId, ms, r =>
        {
            r.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
            r.SseCustomerKey = key;
            r.SseCustomerKeyMd5 = keyMd5;
        }).ConfigureAwait(false);
        Assert.Equal(200, uploadResp1.StatusCode);
        Assert.Equal(SseCustomerAlgorithm.Aes256, uploadResp1.SseCustomerAlgorithm);
        Assert.Equal(keyMd5, uploadResp1.SseCustomerKeyMd5);

        CompleteMultipartUploadResponse completeResp = await client.CompleteMultipartUploadAsync(bucket, objectKey, initResp.UploadId, new[] { uploadResp1 }).ConfigureAwait(false);
        Assert.Equal(200, completeResp.StatusCode);
        Assert.Equal(SseCustomerAlgorithm.Aes256, completeResp.SseCustomerAlgorithm);
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.GoogleCloudStorage)]
    public async Task MultipartLockMode(S3Provider _, string bucket, ISimpleClient client)
    {
        string objectKey = nameof(MultipartLockMode);

        CreateMultipartUploadResponse initResp = await client.CreateMultipartUploadAsync(bucket, objectKey, r =>
        {
            r.LockMode = LockMode.Governance;
            r.LockRetainUntil = DateTimeOffset.UtcNow.AddMinutes(5);
        }).ConfigureAwait(false);
        Assert.Equal(200, initResp.StatusCode);

        byte[] file = new byte[1024 * 1024 * 5];
        await using MemoryStream ms = new MemoryStream(file);

        UploadPartResponse uploadResp = await client.UploadPartAsync(bucket, objectKey, 1, initResp.UploadId, ms, r => r.ContentMd5 = CryptoHelper.Md5Hash(file)).ConfigureAwait(false);
        Assert.Equal(200, uploadResp.StatusCode);

        CompleteMultipartUploadResponse completeResp = await client.CompleteMultipartUploadAsync(bucket, objectKey, initResp.UploadId, new[] { uploadResp }).ConfigureAwait(false);
        Assert.Equal(200, completeResp.StatusCode);
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.GoogleCloudStorage)]
    public async Task MultipartSinglePart(S3Provider provider, string bucket, ISimpleClient client)
    {
        string objectKey = nameof(MultipartSinglePart);

        CreateMultipartUploadResponse createResp = await client.CreateMultipartUploadAsync(bucket, objectKey).ConfigureAwait(false);
        Assert.Equal(200, createResp.StatusCode);
        Assert.Equal(bucket, createResp.BucketName);
        Assert.Equal(objectKey, createResp.ObjectKey);
        Assert.NotNull(createResp.UploadId);

        if (provider == S3Provider.AmazonS3)
        {
            //Test lifecycle expiration
            Assert.Equal(DateTime.UtcNow.AddDays(2).Date, createResp.AbortsOn!.Value.UtcDateTime.Date);
            Assert.Equal("ExpireAll", createResp.AbortRuleId);
        }

        byte[] file = new byte[1024 * 1024 * 5];
        file[0] = (byte)'a';

        await using MemoryStream ms = new MemoryStream(file);

        UploadPartResponse uploadResp = await client.UploadPartAsync(bucket, objectKey, 1, createResp.UploadId, ms).ConfigureAwait(false);
        Assert.Equal(200, uploadResp.StatusCode);
        Assert.Equal("\"10f74ef02085310ccd1f87150b83e537\"", uploadResp.ETag);

        CompleteMultipartUploadResponse completeResp = await client.CompleteMultipartUploadAsync(bucket, objectKey, createResp.UploadId, new[] { uploadResp }).ConfigureAwait(false);
        Assert.Equal(200, completeResp.StatusCode);
        Assert.Equal("\"bd74e21dfa8678d127240f76e518e9c2-1\"", completeResp.ETag);

        if (provider == S3Provider.AmazonS3)
        {
            //Test lifecycle expiration
            Assert.Equal(DateTime.UtcNow.AddDays(2).Date, completeResp.LifeCycleExpiresOn!.Value.UtcDateTime.Date);
            Assert.Equal("ExpireAll", completeResp.LifeCycleRuleId);
        }
    }

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task MultipartTooSmall(S3Provider provider, string bucket, ISimpleClient client)
    {
        string objectKey = nameof(MultipartTooSmall);

        CreateMultipartUploadResponse initResp = await client.CreateMultipartUploadAsync(bucket, objectKey).ConfigureAwait(false);
        Assert.Equal(200, initResp.StatusCode);
        Assert.Equal(bucket, initResp.BucketName);
        Assert.Equal(objectKey, initResp.ObjectKey);
        Assert.NotNull(initResp.UploadId);

        //4 MB is below the 5 MB limit. See https://docs.aws.amazon.com/AmazonS3/latest/dev/qfacts.html
        //Note that if there only is 1 part, then it is technically the last part, and can be of any size. That's why this test has 2 parts.
        byte[] file = new byte[1024 * 1024 * 4];
        byte[][] parts = file.Chunk(file.Length / 2).Select(x => x.ToArray()).ToArray();

        await using MemoryStream ms1 = new MemoryStream(parts[0]);
        UploadPartResponse uploadResp1 = await client.UploadPartAsync(bucket, objectKey, 1, initResp.UploadId, ms1).ConfigureAwait(false);
        Assert.Equal(provider == S3Provider.BackBlazeB2 ? 400 : 200, uploadResp1.StatusCode);

        await using MemoryStream ms2 = new MemoryStream(parts[0]);
        UploadPartResponse uploadResp2 = await client.UploadPartAsync(bucket, objectKey, 2, initResp.UploadId, ms2).ConfigureAwait(false);
        Assert.Equal(provider == S3Provider.BackBlazeB2 ? 400 : 200, uploadResp2.StatusCode);

        CompleteMultipartUploadResponse completeResp = await client.CompleteMultipartUploadAsync(bucket, objectKey, initResp.UploadId, new[] { uploadResp1, uploadResp2 }).ConfigureAwait(false);
        Assert.Equal(provider == S3Provider.BackBlazeB2 ? 500 : 400, completeResp.StatusCode);
    }

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.GoogleCloudStorage)]
    public async Task MultipartUpload(S3Provider provider, string bucket, ISimpleClient client)
    {
        string objectKey = nameof(MultipartUpload);

        CreateMultipartUploadResponse initResp = await client.CreateMultipartUploadAsync(bucket, objectKey).ConfigureAwait(false);
        Assert.Equal(200, initResp.StatusCode);
        Assert.Equal(bucket, initResp.BucketName);
        Assert.Equal(objectKey, initResp.ObjectKey);
        Assert.NotNull(initResp.UploadId);

        byte[] file = new byte[1024 * 1024 * 10];
        file[0] = (byte)'a';
        file[^1] = (byte)'b';

        byte[][] parts = file.Chunk(file.Length / 2).Select(x => x.ToArray()).ToArray();

        await using MemoryStream ms1 = new MemoryStream(parts[0]);
        UploadPartResponse uploadResp1 = await client.UploadPartAsync(bucket, objectKey, 1, initResp.UploadId, ms1).ConfigureAwait(false);
        Assert.Equal(200, uploadResp1.StatusCode);
        Assert.NotNull(uploadResp1.ETag);

        await using MemoryStream ms2 = new MemoryStream(parts[0]);
        UploadPartResponse uploadResp2 = await client.UploadPartAsync(bucket, objectKey, 2, initResp.UploadId, ms2).ConfigureAwait(false);
        Assert.Equal(200, uploadResp2.StatusCode);
        Assert.NotNull(uploadResp2.ETag);

        CompleteMultipartUploadResponse completeResp = await client.CompleteMultipartUploadAsync(bucket, objectKey, initResp.UploadId, new[] { uploadResp1, uploadResp2 }).ConfigureAwait(false);
        Assert.Equal(200, completeResp.StatusCode);
        Assert.NotNull(uploadResp2.ETag);

        if (provider == S3Provider.AmazonS3)
        {
            //Provoke an 'InvalidArgument' error. Parts start from index 1
            GetObjectResponse getResp1 = await client.GetObjectAsync(bucket, nameof(MultipartUpload), r => r.PartNumber = 0).ConfigureAwait(false);
            Assert.Equal(400, getResp1.StatusCode);
            Assert.IsType<InvalidArgumentError>(getResp1.Error);

            GetObjectResponse getResp2 = await client.GetObjectAsync(bucket, nameof(MultipartUpload), r => r.PartNumber = 1).ConfigureAwait(false);
            Assert.Equal(206, getResp2.StatusCode);

            byte[] contentData = await getResp2.Content!.AsDataAsync().ConfigureAwait(false);
            Assert.Equal(parts[0].Length, contentData.Length);
            Assert.Equal(parts[0], contentData);
        }
    }

#if COMMERCIAL
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task MultipartViaClient(S3Provider provider, string bucket, ISimpleClient client)
    {
        byte[] data = new byte[20 * 1024 * 1024]; //20 Mb

        for (int i = 0; i < data.Length; i++)
        {
            data[i] = (byte)(i % 255);
        }

        await using (MemoryStream ms = new MemoryStream(data))
        {
            CompleteMultipartUploadResponse resp = await client.MultipartUploadAsync(bucket, nameof(MultipartViaClient), ms, 5 * 1024 * 1024).ConfigureAwait(false);
            Assert.Equal(200, resp.StatusCode);
        }

        GetObjectResponse getResp = await client.GetObjectAsync(bucket, nameof(MultipartViaClient)).ConfigureAwait(false);
        Assert.Equal(200, getResp.StatusCode);

        await using (MemoryStream ms = new MemoryStream())
        {
            await getResp.Content.CopyToAsync(ms).ConfigureAwait(false);
            Assert.Equal(data, ms.ToArray());
        }

        if (provider == S3Provider.AmazonS3)
        {
            //Try multipart downloading it
            await using (MemoryStream ms = new MemoryStream())
            {
                IAsyncEnumerable<GetObjectResponse> responses = client.MultipartDownloadAsync(bucket, nameof(MultipartViaClient), ms);

                await foreach (GetObjectResponse resp in responses)
                {
                    //We use IsSuccess here since providers return different return code
                    Assert.True(resp.IsSuccess);
                }

                Assert.Equal(data, ms.ToArray());
            }

            HeadObjectResponse headResp = await client.HeadObjectAsync(bucket, nameof(MultipartViaClient), req => req.PartNumber = 1).ConfigureAwait(false);
            Assert.Equal(206, headResp.StatusCode);
            Assert.Equal(4, headResp.NumberOfParts);
        }
    }

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task MultipartViaExtensions(S3Provider _, string bucket, ISimpleClient client)
    {
        byte[] data = new byte[100 * 1024 * 1024]; //100 Mb

        for (int i = 0; i < data.Length; i++)
        {
            data[i] = (byte)'A';
        }

        int count = 0;
        await using (MemoryStream ms = new MemoryStream(data))
        {
            CompleteMultipartUploadResponse uploadResp = await client.MultipartUploadAsync(bucket, nameof(MultipartViaExtensions), ms, 10 * 1024 * 1024, 2, null, response =>
            {
                Assert.Equal(200, response.StatusCode);
                count++;
            });

            Assert.Equal(200, uploadResp.StatusCode);
        }

        Assert.Equal(10, count);
    }
#endif
}