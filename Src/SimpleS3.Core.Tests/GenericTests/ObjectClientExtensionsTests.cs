using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class ObjectClientExtensionsTests
{
    [Fact]
    public async Task ListAllObjectsAsyncDoesNotStartNextRequestWhenEnumerationStops()
    {
        PagingObjectClient client = new PagingObjectClient();

        await foreach (S3Object obj in client.ListAllObjectsAsync("bucket", token: TestContext.Current.CancellationToken))
        {
            Assert.Equal("first", obj.ObjectKey);
            break;
        }

        Assert.Equal(1, client.ListObjectsCalls);
    }

    [Fact]
    public async Task ListAllObjectsAsyncThrowsWhenCanceledBetweenItems()
    {
        PagingObjectClient client = new PagingObjectClient(objectsPerPage: 2);
        using CancellationTokenSource cts = new CancellationTokenSource();
        IAsyncEnumerator<S3Object> enumerator = client.ListAllObjectsAsync("bucket", token: cts.Token).GetAsyncEnumerator();

        try
        {
            Assert.True(await enumerator.MoveNextAsync().AsTask().ConfigureAwait(false));
            await cts.CancelAsync().ConfigureAwait(false);

            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await enumerator.MoveNextAsync().AsTask().ConfigureAwait(false));
        }
        finally
        {
            await enumerator.DisposeAsync().AsTask().ConfigureAwait(false);
        }
    }

    [Fact]
    public async Task ListAllMultipartUploadsAsyncThrowsWhenCanceledBetweenItems()
    {
        StaticMultipartClient client = new StaticMultipartClient();
        using CancellationTokenSource cts = new CancellationTokenSource();
        IAsyncEnumerator<S3Upload> enumerator = client.ListAllMultipartUploadsAsync("bucket", cts.Token).GetAsyncEnumerator();

        try
        {
            Assert.True(await enumerator.MoveNextAsync().AsTask().ConfigureAwait(false));
            await cts.CancelAsync().ConfigureAwait(false);

            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await enumerator.MoveNextAsync().AsTask().ConfigureAwait(false));
        }
        finally
        {
            await enumerator.DisposeAsync().AsTask().ConfigureAwait(false);
        }
    }

    [Fact]
    public async Task ListAllBucketsAsyncThrowsWhenCanceledBetweenItems()
    {
        StaticBucketClient client = new StaticBucketClient();
        using CancellationTokenSource cts = new CancellationTokenSource();
        IAsyncEnumerator<S3Bucket> enumerator = client.ListAllBucketsAsync(token: cts.Token).GetAsyncEnumerator();

        try
        {
            Assert.True(await enumerator.MoveNextAsync().AsTask().ConfigureAwait(false));
            await cts.CancelAsync().ConfigureAwait(false);

            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await enumerator.MoveNextAsync().AsTask().ConfigureAwait(false));
        }
        finally
        {
            await enumerator.DisposeAsync().AsTask().ConfigureAwait(false);
        }
    }

    private sealed class PagingObjectClient(int objectsPerPage = 1) : IObjectClient
    {
        private int _listObjectsCalls;

        public int ListObjectsCalls => Volatile.Read(ref _listObjectsCalls);

        public Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest>? config = null, CancellationToken token = default)
        {
            Interlocked.Increment(ref _listObjectsCalls);

            ListObjectsResponse response = new ListObjectsResponse
            {
                IsSuccess = true,
                IsTruncated = true,
                NextContinuationToken = "next"
            };

            response.Objects.Add(new S3Object("first", DateTimeOffset.UnixEpoch, 0, null, null, default, default, default));

            if (objectsPerPage > 1)
                response.Objects.Add(new S3Object("second", DateTimeOffset.UnixEpoch, 0, null, null, default, default, default));

            return Task.FromResult(response);
        }

        public Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream? data, Action<PutObjectRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<RestoreObjectResponse> RestoreObjectAsync(string bucketName, string objectKey, Action<RestoreObjectRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<CopyObjectResponse> CopyObjectAsync(string sourceBucketName, string sourceObjectKey, string destinationBucket, string destinationObjectKey, Action<CopyObjectRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutObjectAclResponse> PutObjectAclAsync(string bucketName, string objectKey, Action<PutObjectAclRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetObjectAclResponse> GetObjectAclAsync(string bucketName, string objectKey, Action<GetObjectAclRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetObjectLegalHoldResponse> GetObjectLegalHoldAsync(string bucketName, string objectKey, Action<GetObjectLegalHoldRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutObjectLegalHoldResponse> PutObjectLegalHoldAsync(string bucketName, string objectKey, bool lockStatus, Action<PutObjectLegalHoldRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<ListObjectVersionsResponse> ListObjectVersionsAsync(string bucketName, Action<ListObjectVersionsRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
    }

    private sealed class StaticMultipartClient : IMultipartClient
    {
        public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest>? config = null, CancellationToken token = default)
        {
            ListMultipartUploadsResponse response = new ListMultipartUploadsResponse { IsSuccess = true };
            response.Uploads.Add(new S3Upload("first", "upload-1", null, null, default, DateTimeOffset.UnixEpoch));
            response.Uploads.Add(new S3Upload("second", "upload-2", null, null, default, DateTimeOffset.UnixEpoch));
            return Task.FromResult(response);
        }

        public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<S3PartInfo> parts, Action<CompleteMultipartUploadRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
    }

    private sealed class StaticBucketClient : IBucketClient
    {
        public Task<ListBucketsResponse> ListBucketsAsync(Action<ListBucketsRequest>? config = null, CancellationToken token = default)
        {
            ListBucketsResponse response = new ListBucketsResponse { IsSuccess = true };
            response.Buckets.Add(new S3Bucket("first", DateTimeOffset.UnixEpoch));
            response.Buckets.Add(new S3Bucket("second", DateTimeOffset.UnixEpoch));
            return Task.FromResult(response);
        }

        public Task<CreateBucketResponse> CreateBucketAsync(string bucketName, Action<CreateBucketRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeleteBucketResponse> DeleteBucketAsync(string bucketName, Action<DeleteBucketRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<HeadBucketResponse> HeadBucketAsync(string bucketName, Action<HeadBucketRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutBucketLockConfigurationResponse> PutBucketLockConfigurationAsync(string bucketName, bool enabled, Action<PutBucketLockConfigurationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetBucketLockConfigurationResponse> GetBucketLockConfigurationAsync(string bucketName, Action<GetBucketLockConfigurationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetBucketTaggingResponse> GetBucketTaggingAsync(string bucketName, Action<GetBucketTaggingRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutBucketTaggingResponse> PutBucketTaggingAsync(string bucketName, IDictionary<string, string> tags, Action<PutBucketTaggingRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeleteBucketTaggingResponse> DeleteBucketTaggingAsync(string bucketName, Action<DeleteBucketTaggingRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutBucketAccelerateConfigurationResponse> PutBucketAccelerateConfigurationAsync(string bucketName, bool enabled, Action<PutBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetBucketAccelerateConfigurationResponse> GetBucketAccelerateConfigurationAsync(string bucketName, Action<GetBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutBucketLifecycleConfigurationResponse> PutBucketLifecycleConfigurationAsync(string bucketName, IEnumerable<S3Rule> rules, Action<PutBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutBucketEncryptionResponse> PutBucketEncryptionAsync(string bucketName, IEnumerable<S3ServerSideEncryptionRule> rules, Action<PutBucketEncryptionRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutBucketVersioningResponse> PutBucketVersioningAsync(string bucketName, bool enabled, Action<PutBucketVersioningRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetBucketVersioningResponse> GetBucketVersioningAsync(string bucketName, Action<GetBucketVersioningRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetBucketLifecycleConfigurationResponse> GetBucketLifecycleConfigurationAsync(string bucketName, Action<GetBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutPublicAccessBlockResponse> PutPublicAccessBlockAsync(string bucketName, Action<PutPublicAccessBlockRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetBucketPolicyResponse> GetBucketPolicyAsync(string bucketName, Action<GetBucketPolicyRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeleteBucketPolicyResponse> DeleteBucketPolicyAsync(string bucketName, Action<DeleteBucketPolicyRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutBucketPolicyResponse> PutBucketPolicyAsync(string bucketName, string policy, Action<PutBucketPolicyRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
    }
}