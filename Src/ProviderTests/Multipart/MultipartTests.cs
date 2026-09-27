using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Multipart;

public class MultipartTests : TestBase
{
    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, SseAlgorithm.Aes256, SseAlgorithm.AwsKms)]
    [MultipleProvidersWithData(S3Provider.GoogleCloudStorage, SseAlgorithm.Aes256)]
    public async Task MultipartWithEncryption(S3Provider provider, string _, ISimpleClient client, SseAlgorithm algorithm) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        const string objectKey = nameof(MultipartWithEncryption);

        CreateMultipartUploadResponse createResp = await client.CreateMultipartUploadAsync(bucket, objectKey, req => req.SseAlgorithm = algorithm, TestContext.Current.CancellationToken);
        Assert.Equal(200, createResp.StatusCode);

        if (provider == S3Provider.AmazonS3)
            Assert.Equal(algorithm, createResp.SseAlgorithm);

        if (algorithm == SseAlgorithm.AwsKms)
            Assert.NotNull(createResp.SseKmsKeyId);

        byte[] data = new byte[4096];
        UploadPartResponse uploadResp = await UploadTestPartAsync(client, bucket, objectKey, createResp.UploadId, 1, data);

        if (provider == S3Provider.AmazonS3)
            Assert.Equal(algorithm, uploadResp.SseAlgorithm);

        if (algorithm == SseAlgorithm.AwsKms)
            Assert.NotNull(uploadResp.SseKmsKeyId);

        CompleteMultipartUploadResponse completeResp = await client.CompleteMultipartUploadAsync(bucket, objectKey, createResp.UploadId, [new S3PartInfo(uploadResp.ETag, 1)], token: TestContext.Current.CancellationToken);
        Assert.Equal(200, completeResp.StatusCode);

        if (provider == S3Provider.AmazonS3)
            Assert.Equal(algorithm, completeResp.SseAlgorithm);

        if (algorithm == SseAlgorithm.AwsKms)
            Assert.NotNull(completeResp.SseKmsKeyId);
        await AssertObjectContentAsync(client, bucket, objectKey, data);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task MultipartCustomerEncryption(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        const string objectKey = nameof(MultipartCustomerEncryption);

        await AllowSseCustomerKeysAsync(provider, client, bucket);

        byte[] key = [1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8];
        byte[] keyMd5 = CryptoHelper.Md5Hash(key);

        CreateMultipartUploadResponse initResp = await client.CreateMultipartUploadAsync(bucket, objectKey, r =>
        {
            r.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
            r.SseCustomerKey = key;
            r.SseCustomerKeyMd5 = keyMd5;
        }, TestContext.Current.CancellationToken);
        Assert.Equal(200, initResp.StatusCode);

        byte[] data = new byte[4096];
        UploadPartResponse uploadResp1 = await UploadTestPartAsync(client, bucket, objectKey, initResp.UploadId, 1, data, r =>
        {
            r.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
            r.SseCustomerKey = key;
            r.SseCustomerKeyMd5 = keyMd5;
        });
        Assert.Equal(SseCustomerAlgorithm.Aes256, uploadResp1.SseCustomerAlgorithm);
        Assert.Equal(keyMd5, uploadResp1.SseCustomerKeyMd5);

        CompleteMultipartUploadResponse completeResp = await client.CompleteMultipartUploadAsync(bucket, objectKey, initResp.UploadId, [new S3PartInfo(uploadResp1.ETag, 1)], token: TestContext.Current.CancellationToken);
        Assert.Equal(200, completeResp.StatusCode);
        Assert.Equal(SseCustomerAlgorithm.Aes256, completeResp.SseCustomerAlgorithm);
        await AssertObjectContentAsync(client, bucket, objectKey, data, request =>
        {
            request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
            request.SseCustomerKey = key;
            request.SseCustomerKeyMd5 = keyMd5;
        });
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.GoogleCloudStorage)]
    public async Task MultipartLockMode(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        string objectKey = nameof(MultipartLockMode) + "-" + Guid.NewGuid();
        DateTimeOffset retainUntil = DateTimeOffset.UtcNow.AddSeconds(5);

        CreateMultipartUploadResponse initResp = await client.CreateMultipartUploadAsync(bucket, objectKey, r =>
        {
            r.LockMode = LockMode.Governance;
            r.LockRetainUntil = retainUntil;
        }, TestContext.Current.CancellationToken);
        Assert.Equal(200, initResp.StatusCode);

        byte[] file = new byte[4096];
        UploadPartResponse uploadResp = await UploadTestPartAsync(client, bucket, objectKey, initResp.UploadId, 1, file, r => r.ContentMd5 = CryptoHelper.Md5Hash(file));

        CompleteMultipartUploadResponse completeResp = await client.CompleteMultipartUploadAsync(bucket, objectKey, initResp.UploadId, [new S3PartInfo(uploadResp.ETag, 1)], token: TestContext.Current.CancellationToken);
        Assert.Equal(200, completeResp.StatusCode);

        HeadObjectResponse head = await client.HeadObjectAsync(bucket, objectKey, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, head.StatusCode);
        Assert.Equal(LockMode.Governance, head.LockMode);
        if (provider == S3Provider.GoogleCloudStorage)
            Assert.Equal(DateTimeOffset.MinValue, head.LockRetainUntil); // GCS reports a default timestamp through its XML API.
        else
            Assert.Equal(retainUntil.UtcDateTime, head.LockRetainUntil!.Value.UtcDateTime, TimeSpan.FromSeconds(1));

        TimeSpan retentionRemaining = retainUntil - DateTimeOffset.UtcNow;

        if (retentionRemaining > TimeSpan.Zero)
            await Task.Delay(retentionRemaining + TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);

        DeleteObjectResponse deleteResp = await client.DeleteObjectAsync(bucket, objectKey, request =>
        {
            request.VersionId = completeResp.VersionId;
            request.BypassGovernanceRetention = provider == S3Provider.AmazonS3;
        }, TestContext.Current.CancellationToken);
        Assert.Equal(204, deleteResp.StatusCode);
    }, request => request.EnableObjectLocking = true);

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.GoogleCloudStorage)]
    public async Task SingleFinalPartMayBeSmallerThanMinimum(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        const string objectKey = nameof(SingleFinalPartMayBeSmallerThanMinimum);

        CreateMultipartUploadResponse createResp = await client.CreateMultipartUploadAsync(bucket, objectKey, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, createResp.StatusCode);
        Assert.Equal(bucket, createResp.BucketName);
        Assert.Equal(objectKey, createResp.ObjectKey);
        Assert.NotNull(createResp.UploadId);

        byte[] file = new byte[4096];
        file[0] = (byte)'a';

        UploadPartResponse uploadResp = await UploadTestPartAsync(client, bucket, objectKey, createResp.UploadId, 1, file);
        Assert.NotNull(uploadResp.ETag);

        CompleteMultipartUploadResponse completeResp = await client.CompleteMultipartUploadAsync(bucket, objectKey, createResp.UploadId, [new S3PartInfo(uploadResp.ETag, 1)], token: TestContext.Current.CancellationToken);
        Assert.Equal(200, completeResp.StatusCode);
        await AssertObjectContentAsync(client, bucket, objectKey, file);
    });

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task CompletionRejectsUndersizedNonfinalPart(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        const string objectKey = nameof(CompletionRejectsUndersizedNonfinalPart);

        CreateMultipartUploadResponse initResp = await client.CreateMultipartUploadAsync(bucket, objectKey, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, initResp.StatusCode);
        Assert.Equal(bucket, initResp.BucketName);
        Assert.Equal(objectKey, initResp.ObjectKey);
        Assert.NotNull(initResp.UploadId);

        //4 MB is below the 5 MB limit. See https://docs.aws.amazon.com/AmazonS3/latest/dev/qfacts.html
        //Note that if there only is 1 part, then it is technically the last part, and can be of any size. That's why this test has 2 parts.
        byte[] file = new byte[1024 * 1024 * 4];
        byte[][] parts = file.Chunk(file.Length / 2).Select(x => x.ToArray()).ToArray();

        UploadPartResponse uploadResp1 = await UploadTestPartAsync(client, bucket, objectKey, initResp.UploadId, 1, parts[0]);
        UploadPartResponse uploadResp2 = await UploadTestPartAsync(client, bucket, objectKey, initResp.UploadId, 2, parts[1]);

        CompleteMultipartUploadResponse completeResp = await client.CompleteMultipartUploadAsync(bucket, objectKey, initResp.UploadId, [new S3PartInfo(uploadResp1.ETag, 1), new S3PartInfo(uploadResp2.ETag, 2)], token: TestContext.Current.CancellationToken);
        Assert.Equal(400, completeResp.StatusCode);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.GoogleCloudStorage)]
    public async Task CompleteMultipartUploadRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        const string objectKey = nameof(CompleteMultipartUploadRoundTrip);

        CreateMultipartUploadResponse initResp = await client.CreateMultipartUploadAsync(bucket, objectKey, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, initResp.StatusCode);
        Assert.Equal(bucket, initResp.BucketName);
        Assert.Equal(objectKey, initResp.ObjectKey);
        Assert.NotNull(initResp.UploadId);

        byte[] file = new byte[1024 * 1024 * 10];
        file[0] = (byte)'a';
        file[^1] = (byte)'b';

        byte[][] parts = file.Chunk(file.Length / 2).Select(x => x.ToArray()).ToArray();

        UploadPartResponse uploadResp1 = await UploadTestPartAsync(client, bucket, objectKey, initResp.UploadId, 1, parts[0]);
        Assert.NotNull(uploadResp1.ETag);

        UploadPartResponse uploadResp2 = await UploadTestPartAsync(client, bucket, objectKey, initResp.UploadId, 2, parts[1]);
        Assert.NotNull(uploadResp2.ETag);

        CompleteMultipartUploadResponse completeResp = await client.CompleteMultipartUploadAsync(bucket, objectKey, initResp.UploadId, [new S3PartInfo(uploadResp1.ETag, 1), new S3PartInfo(uploadResp2.ETag, 2)], token: TestContext.Current.CancellationToken);
        Assert.Equal(200, completeResp.StatusCode);
        Assert.NotNull(completeResp.ETag);

        await AssertObjectContentAsync(client, bucket, objectKey, file);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task MultipartUploadRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        byte[] data = new byte[(5 * 1024 * 1024) + 1024];

        for (int i = 0; i < data.Length; i++)
            data[i] = (byte)(i % 255);

        await using (MemoryStream ms = new MemoryStream(data))
        {
            CompleteMultipartUploadResponse resp = await client.MultipartUploadAsync(bucket, nameof(MultipartUploadRoundTrip), ms, 5 * 1024 * 1024, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, resp.StatusCode);
        }

        await AssertObjectContentAsync(client, bucket, nameof(MultipartUploadRoundTrip), data);
    });

    [Theory]
    [MultipleProviders(S3Provider.All)]
    public async Task MultipartUploadInvokesCallbackForEachPart(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        byte[] data = new byte[10 * 1024 * 1024];

        for (int i = 0; i < data.Length; i++)
            data[i] = (byte)'A';

        int count = 0;

        await using (MemoryStream ms = new MemoryStream(data))
        {
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            CompleteMultipartUploadResponse uploadResp = await client.MultipartUploadAsync(bucket, nameof(MultipartUploadInvokesCallbackForEachPart), ms, 5 * 1024 * 1024, 2, null, response =>
            {
                Assert.Equal(200, response.StatusCode);
                Interlocked.Increment(ref count);
            }, TestContext.Current.CancellationToken);

            Assert.Equal(200, uploadResp.StatusCode);
        }

        Assert.Equal(2, count);
        await AssertObjectContentAsync(client, bucket, nameof(MultipartUploadInvokesCallbackForEachPart), data);
    });

    [SuppressMessage("Security", "CA5350", Justification = "S3 requires this legacy hash for its wire-level checksum; it is not used for password storage or a new signature scheme.")]
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task MultipartChecksum(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        const string objectKey = nameof(MultipartChecksum);

        CreateMultipartUploadResponse initResp = await client.CreateMultipartUploadAsync(bucket, objectKey, r =>
        {
            r.ChecksumAlgorithm = ChecksumAlgorithm.Sha1;
            r.ChecksumType = ChecksumType.Composite;
        }, TestContext.Current.CancellationToken);

        Assert.Equal(200, initResp.StatusCode);
        Assert.Equal(ChecksumType.Composite, initResp.ChecksumType);
        Assert.Equal(ChecksumAlgorithm.Sha1, initResp.ChecksumAlgorithm);

        byte[] data = "hello world"u8.ToArray();
        byte[] checksum = SHA1.HashData(data);
        UploadPartResponse uploadResp = await UploadTestPartAsync(client, bucket, objectKey, initResp.UploadId, 1, data, r =>
        {
            r.ChecksumAlgorithm = ChecksumAlgorithm.Sha1;
            r.Checksum = checksum;
        });
        Assert.Equal(ChecksumAlgorithm.Sha1, uploadResp.ChecksumAlgorithm);
        Assert.Equal(checksum, uploadResp.Checksum);

        CompleteMultipartUploadResponse completeResp = await client.CompleteMultipartUploadAsync(bucket, objectKey, initResp.UploadId, [new S3PartInfo(uploadResp.ETag, 1, checksum, ChecksumAlgorithm.Sha1)], token: TestContext.Current.CancellationToken);
        Assert.Equal(200, completeResp.StatusCode);
        Assert.Equal(ChecksumType.Composite, completeResp.ChecksumType);
        Assert.Equal(ChecksumAlgorithm.Sha1, completeResp.ChecksumAlgorithm);
        Assert.NotNull(completeResp.Checksum);
        await AssertObjectContentAsync(client, bucket, objectKey, data);
    });
}