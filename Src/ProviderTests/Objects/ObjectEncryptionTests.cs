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
    [MultipleProvidersWithData(S3Provider.AmazonS3, 0, 1, 2, 3, 4, 5)]
    public async Task ExplicitBucketKeySelectionAppliesToWriteShapes(S3Provider _, string bucket, ISimpleClient client, int testCase)
    {
        int shape = testCase / 2;
        bool enabled = testCase % 2 == 1;
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

            if (shape == 0)
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
            else if (shape == 1)
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
}