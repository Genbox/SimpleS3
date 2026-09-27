using System.Net;
using System.Text;
using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Objects;

public class ObjectEncryptionTests : TestBase
{
    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, false, true)]
    public async Task PutObjectBucketKey(S3Provider _, string bucket, ISimpleClient client, bool enabled) => await WriteWithBucketKeyAsync(bucket, client, WriteOperation.Put, enabled);

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, false, true)]
    public async Task CopyObjectBucketKey(S3Provider _, string bucket, ISimpleClient client, bool enabled) => await WriteWithBucketKeyAsync(bucket, client, WriteOperation.Copy, enabled);

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, false, true)]
    public async Task MultipartBucketKey(S3Provider _, string bucket, ISimpleClient client, bool enabled) => await WriteWithBucketKeyAsync(bucket, client, WriteOperation.Multipart, enabled);

    private static async Task WriteWithBucketKeyAsync(string bucket, ISimpleClient client, WriteOperation operation, bool enabled)
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        string prefix = "explicit-bucket-key-" + Guid.NewGuid().ToString("N");
        string sourceKey = prefix + "-source";
        string destinationKey = prefix + "-destination";
        string? uploadId = null;
        bool completed = false;
        bool? expectedResponseValue = enabled ? true : null;
        const string payload = "explicit bucket key payload";

        try
        {
            bool? writeBucketKey;

            if (operation == WriteOperation.Put)
            {
                PutObjectResponse response = await client.PutObjectStringAsync(bucket, destinationKey, payload, config: request =>
                {
                    request.SseAlgorithm = SseAlgorithm.AwsKms;
                    request.SseKmsKeyId = "alias/aws/s3";
                    request.BucketKeyEnabled = enabled;
                }, token: token);
                Assert.True(response.IsSuccess, response.Error?.Message);
                writeBucketKey = response.BucketKeyEnabled;
            }
            else if (operation == WriteOperation.Copy)
            {
                PutObjectResponse source = await client.PutObjectStringAsync(bucket, sourceKey, payload, token: token);
                Assert.True(source.IsSuccess, source.Error?.Message);
                CopyObjectResponse response = await client.CopyObjectAsync(bucket, sourceKey, bucket, destinationKey, request =>
                {
                    request.SseAlgorithm = SseAlgorithm.AwsKms;
                    request.SseKmsKeyId = "alias/aws/s3";
                    request.BucketKeyEnabled = enabled;
                }, token);
                Assert.True(response.IsSuccess, response.Error?.Message);
                writeBucketKey = response.BucketKeyEnabled;
            }
            else
            {
                CreateMultipartUploadResponse initiated = await client.CreateMultipartUploadAsync(bucket, destinationKey, request =>
                {
                    request.SseAlgorithm = SseAlgorithm.AwsKms;
                    request.SseKmsKeyId = "alias/aws/s3";
                    request.BucketKeyEnabled = enabled;
                }, token);
                Assert.True(initiated.IsSuccess, initiated.Error?.Message);
                Assert.Equal(expectedResponseValue, initiated.BucketKeyEnabled);
                uploadId = Assert.IsType<string>(initiated.UploadId);
                await using MemoryStream content = new MemoryStream(Encoding.UTF8.GetBytes(payload));
                UploadPartResponse part = await client.UploadPartAsync(bucket, destinationKey, 1, uploadId, content, token: token);
                Assert.True(part.IsSuccess, part.Error?.Message);
                Assert.NotNull(part.ETag);
                CompleteMultipartUploadResponse response = await client.CompleteMultipartUploadAsync(bucket, destinationKey, uploadId, [new S3PartInfo(part.ETag, 1)], token: token);
                Assert.True(response.IsSuccess, response.Error?.Message);
                completed = true;
                writeBucketKey = response.BucketKeyEnabled;
            }

            Assert.Equal(expectedResponseValue, writeBucketKey);
            HeadObjectResponse head = await client.HeadObjectAsync(bucket, destinationKey, token: token);
            Assert.True(head.IsSuccess, head.Error?.Message);
            Assert.Equal(SseAlgorithm.AwsKms, head.SseAlgorithm);
            Assert.Equal(expectedResponseValue, head.BucketKeyEnabled);
            using GetObjectResponse get = await client.GetObjectAsync(bucket, destinationKey, token: token);
            Assert.True(get.IsSuccess, get.Error?.Message);
            Assert.Equal(expectedResponseValue, get.BucketKeyEnabled);
            Assert.Equal(payload, await get.Content.AsStringAsync(token: token));
        }
        finally
        {
            using CancellationTokenSource cleanup = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            if (uploadId != null && !completed)
                await client.AbortMultipartUploadAsync(bucket, destinationKey, uploadId, token: cleanup.Token);
            ListObjectVersionsResponse versions = await client.ListObjectVersionsAsync(bucket, request => request.Prefix = prefix, cleanup.Token);
            Assert.True(versions.IsSuccess, versions.Error?.Message);
            foreach (S3Version version in versions.Versions.Where(version => version.ObjectKey is not null && version.ObjectKey.StartsWith(prefix, StringComparison.Ordinal)))
                Assert.Equal(204, (await client.DeleteObjectAsync(bucket, version.ObjectKey, request => request.VersionId = version.VersionId, cleanup.Token)).StatusCode);
            foreach (S3DeleteMarker marker in versions.DeleteMarkers.Where(marker => marker.ObjectKey is not null && marker.ObjectKey.StartsWith(prefix, StringComparison.Ordinal)))
                Assert.Equal(204, (await client.DeleteObjectAsync(bucket, marker.ObjectKey, request => request.VersionId = marker.VersionId, cleanup.Token)).StatusCode);
            Assert.Equal(404, (await client.HeadObjectAsync(bucket, sourceKey, token: cleanup.Token)).StatusCode);
            Assert.Equal(404, (await client.HeadObjectAsync(bucket, destinationKey, token: cleanup.Token)).StatusCode);
        }
    }

    [Theory]
    [SingleProvider(S3Provider.AmazonS3)]
    public async Task UpdateObjectEncryptionRoundTrip(S3Provider provider, string _, ISimpleClient client) => await UpdateEncryptionAsync(provider, client, null);

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, false, true)]
    public async Task UpdateObjectEncryptionBucketKey(S3Provider provider, string _, ISimpleClient client, bool enabled) => await UpdateEncryptionAsync(provider, client, enabled);

    private static async Task UpdateEncryptionAsync(S3Provider provider, ISimpleClient client, bool? bucketKeyEnabled)
    {
        const string kmsKeyId = "alias/aws/s3";
        await CreateTempBucketAsync(provider, client, async tempBucket =>
        {
            const string objectKey = "kms-object";
            PutObjectResponse putResp = await client.PutObjectAsync(tempBucket, objectKey, null, request =>
            {
                request.SseAlgorithm = SseAlgorithm.AwsKms;
                request.SseKmsKeyId = kmsKeyId;
            }, TestContext.Current.CancellationToken);
            Assert.Equal(200, putResp.StatusCode);

            Assert.NotNull(putResp.SseKmsKeyId);
            Assert.StartsWith("arn:", putResp.SseKmsKeyId, StringComparison.Ordinal);

            PutObjectResponse source = await client.PutObjectStringAsync(tempBucket, objectKey, "encryption-round-trip", config: request => request.SseAlgorithm = SseAlgorithm.Aes256, token: TestContext.Current.CancellationToken);
            Assert.True(source.IsSuccess);
            Assert.Equal(SseAlgorithm.Aes256, source.SseAlgorithm);
            UpdateObjectEncryptionResponse updateResp = await client.UpdateObjectEncryptionAsync(tempBucket, objectKey, putResp.SseKmsKeyId, request => request.BucketKeyEnabled = bucketKeyEnabled, TestContext.Current.CancellationToken);
            Assert.True(updateResp.IsSuccess, updateResp.Error?.Message);

            HeadObjectResponse headResp = await client.HeadObjectAsync(tempBucket, objectKey, token: TestContext.Current.CancellationToken);
            Assert.Equal(200, headResp.StatusCode);
            Assert.Equal(SseAlgorithm.AwsKms, headResp.SseAlgorithm);
            Assert.Equal(putResp.SseKmsKeyId, headResp.SseKmsKeyId);
            Assert.Equal(source.ETag, headResp.ETag);
            using HttpClient http = new HttpClient();
            string signedHead = client.SignRequest(new HeadObjectRequest(tempBucket, objectKey), TimeSpan.FromMinutes(2));
            using HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Head, signedHead);
            using HttpResponseMessage rawHead = await http.SendAsync(message, TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, rawHead.StatusCode);
            bool hasBucketKey = rawHead.Headers.TryGetValues("x-amz-server-side-encryption-bucket-key-enabled", out IEnumerable<string>? bucketKeyValues);

            if (bucketKeyEnabled == true)
            {
                Assert.True(hasBucketKey);
                Assert.Equal("true", Assert.Single(bucketKeyValues!));
            }
            else if (hasBucketKey)
                Assert.Equal("false", Assert.Single(bucketKeyValues!));

            using GetObjectResponse content = await client.GetObjectAsync(tempBucket, objectKey, token: TestContext.Current.CancellationToken);
            Assert.True(content.IsSuccess);
            Assert.Equal("encryption-round-trip", await content.Content.AsStringAsync(token: TestContext.Current.CancellationToken));
        });
    }

    private enum WriteOperation
    {
        Put,
        Copy,
        Multipart
    }
}