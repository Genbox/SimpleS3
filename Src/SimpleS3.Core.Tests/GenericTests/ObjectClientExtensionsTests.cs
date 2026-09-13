using System.Diagnostics.CodeAnalysis;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Enums;
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

[SuppressMessage("Usage", "S5034", Justification = "Each MoveNextAsync call creates a distinct ValueTask that is converted to a Task exactly once before it is awaited.")]
public class ObjectClientExtensionsTests
{
    [Fact]
    public async Task ListAllObjectsAsyncCancelsPrefetchedNextPageWhenEnumerationStops()
    {
        PagingObjectClient client = new PagingObjectClient(2, true);
        IAsyncEnumerator<S3Object> enumerator = client.ListAllObjectsAsync("bucket", token: TestContext.Current.CancellationToken).GetAsyncEnumerator(TestContext.Current.CancellationToken);

        try
        {
            Assert.True(await enumerator.MoveNextAsync().AsTask().WaitAsync(TestContext.Current.CancellationToken));
            Assert.Equal("first", enumerator.Current.ObjectKey);

            await client.PrefetchStarted.WaitAsync(TestContext.Current.CancellationToken);
            Assert.Equal(2, client.ListObjectsCalls);
        }
        finally
        {
            await enumerator.DisposeAsync().AsTask().WaitAsync(TestContext.Current.CancellationToken);
        }

        await client.PrefetchCanceled.WaitAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ListAllObjectsAsyncThrowsWhenCanceledBetweenItems()
    {
        PagingObjectClient client = new PagingObjectClient(objectsPerPage: 2);
        using CancellationTokenSource cts = new CancellationTokenSource();
        IAsyncEnumerator<S3Object> enumerator = client.ListAllObjectsAsync("bucket", token: cts.Token).GetAsyncEnumerator(cts.Token);

        try
        {
            Assert.True(await enumerator.MoveNextAsync().AsTask());
            await cts.CancelAsync();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await enumerator.MoveNextAsync().AsTask());
        }
        finally
        {
            await enumerator.DisposeAsync().AsTask();
        }
    }

    [Fact]
    public async Task ListAllMultipartUploadsAsyncThrowsWhenCanceledBetweenItems()
    {
        StaticMultipartClient client = new StaticMultipartClient();
        using CancellationTokenSource cts = new CancellationTokenSource();
        IAsyncEnumerator<S3Upload> enumerator = client.ListAllMultipartUploadsAsync("bucket", cts.Token).GetAsyncEnumerator(cts.Token);

        try
        {
            Assert.True(await enumerator.MoveNextAsync().AsTask());
            await cts.CancelAsync();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await enumerator.MoveNextAsync().AsTask());
        }
        finally
        {
            await enumerator.DisposeAsync().AsTask();
        }
    }

    [Fact]
    public async Task ListAllMultipartUploadsAsyncUsesBothPaginationMarkers()
    {
        PagingMultipartClient client = new PagingMultipartClient();

        List<S3Upload> uploads = await CollectAsync(client.ListAllMultipartUploadsAsync("bucket", TestContext.Current.CancellationToken), TestContext.Current.CancellationToken);

        Assert.Equal(2, uploads.Count);
        Assert.Equal("next-key", client.SecondRequestKeyMarker);
        Assert.Equal("next-upload", client.SecondRequestUploadIdMarker);
    }

    [Fact]
    public async Task ListAllMultipartUploadsAsyncThrowsOnFailedPage()
    {
        PagingMultipartClient client = new PagingMultipartClient(failFirstPage: true);

        await Assert.ThrowsAsync<S3RequestException>(async () => await CollectAsync(client.ListAllMultipartUploadsAsync("bucket", TestContext.Current.CancellationToken), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ListAllObjectVersionsAsyncUsesVersionIdMarker()
    {
        PagingObjectClient client = new PagingObjectClient(versionPages: true);

        List<S3Version> versions = await CollectAsync(client.ListAllObjectVersionsAsync("bucket", token: TestContext.Current.CancellationToken), TestContext.Current.CancellationToken);

        Assert.Equal(2, versions.Count);
        Assert.Equal("next-key", client.SecondVersionRequestKeyMarker);
        Assert.Equal("next-version", client.SecondVersionRequestVersionIdMarker);
    }

    [Fact]
    public async Task ListAllBucketsAsyncThrowsWhenCanceledBetweenItems()
    {
        StaticBucketClient client = new StaticBucketClient();
        using CancellationTokenSource cts = new CancellationTokenSource();
        IAsyncEnumerator<S3Bucket> enumerator = client.ListAllBucketsAsync(token: cts.Token).GetAsyncEnumerator(cts.Token);

        try
        {
            Assert.True(await enumerator.MoveNextAsync().AsTask());
            await cts.CancelAsync();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await enumerator.MoveNextAsync().AsTask());
        }
        finally
        {
            await enumerator.DisposeAsync().AsTask();
        }
    }

    private static async Task<List<T>> CollectAsync<T>(IAsyncEnumerable<T> source, CancellationToken token)
    {
        List<T> result = new List<T>();

        await foreach (T item in source.WithCancellation(token))
            result.Add(item);

        return result;
    }

    private sealed class PagingObjectClient : IObjectClient
    {
        private readonly bool _blockSecondRequest;
        private readonly int _objectsPerPage;
        private readonly TaskCompletionSource _prefetchCanceled = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource _prefetchStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly bool _versionPages;
        private int _listObjectVersionsCalls;
        private int _listObjectsCalls;

        public PagingObjectClient(int objectsPerPage = 1, bool blockSecondRequest = false, bool versionPages = false)
        {
            _objectsPerPage = objectsPerPage;
            _blockSecondRequest = blockSecondRequest;
            _versionPages = versionPages;
        }

        public int ListObjectsCalls => Volatile.Read(ref _listObjectsCalls);
        public Task PrefetchStarted => _prefetchStarted.Task;
        public Task PrefetchCanceled => _prefetchCanceled.Task;
        public string? SecondVersionRequestKeyMarker { get; private set; }
        public string? SecondVersionRequestVersionIdMarker { get; private set; }

        public async Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest>? config = null, CancellationToken token = default)
        {
            int call = Interlocked.Increment(ref _listObjectsCalls);

            if (_blockSecondRequest && call == 2)
            {
                _prefetchStarted.SetResult();

                try
                {
                    await Task.Delay(Timeout.InfiniteTimeSpan, token);
                }
                catch (OperationCanceledException) when (token.IsCancellationRequested)
                {
                    _prefetchCanceled.SetResult();
                    throw;
                }
            }

            ListObjectsResponse response = new ListObjectsResponse
            {
                IsSuccess = true,
                IsTruncated = true,
                NextContinuationToken = "next"
            };

            response.Objects.Add(new S3Object("first", DateTimeOffset.UnixEpoch, 0, null, null, default, default, default));

            if (_objectsPerPage > 1)
                response.Objects.Add(new S3Object("second", DateTimeOffset.UnixEpoch, 0, null, null, default, default, default));

            return response;
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
        public Task<GetObjectAttributesResponse> GetObjectAttributesAsync(string bucketName, string objectKey, IEnumerable<ObjectAttribute> objectAttributes, Action<GetObjectAttributesRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetObjectAnnotationResponse> GetObjectAnnotationAsync(string bucketName, string objectKey, string annotationName, Action<GetObjectAnnotationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutObjectAnnotationResponse> PutObjectAnnotationAsync(string bucketName, string objectKey, string annotationName, Stream content, Action<PutObjectAnnotationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeleteObjectAnnotationResponse> DeleteObjectAnnotationAsync(string bucketName, string objectKey, string annotationName, Action<DeleteObjectAnnotationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<ListObjectAnnotationsResponse> ListObjectAnnotationsAsync(string bucketName, string objectKey, Action<ListObjectAnnotationsRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetObjectLegalHoldResponse> GetObjectLegalHoldAsync(string bucketName, string objectKey, Action<GetObjectLegalHoldRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutObjectLegalHoldResponse> PutObjectLegalHoldAsync(string bucketName, string objectKey, bool lockStatus, Action<PutObjectLegalHoldRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetObjectTaggingResponse> GetObjectTaggingAsync(string bucketName, string objectKey, Action<GetObjectTaggingRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutObjectTaggingResponse> PutObjectTaggingAsync(string bucketName, string objectKey, IDictionary<string, string> tags, Action<PutObjectTaggingRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeleteObjectTaggingResponse> DeleteObjectTaggingAsync(string bucketName, string objectKey, Action<DeleteObjectTaggingRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetObjectRetentionResponse> GetObjectRetentionAsync(string bucketName, string objectKey, Action<GetObjectRetentionRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutObjectRetentionResponse> PutObjectRetentionAsync(string bucketName, string objectKey, LockMode lockMode, DateTimeOffset retainUntil, Action<PutObjectRetentionRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();

        public Task<ListObjectVersionsResponse> ListObjectVersionsAsync(string bucketName, Action<ListObjectVersionsRequest>? config = null, CancellationToken token = default)
        {
            if (!_versionPages)
                throw new NotSupportedException();

            int call = Interlocked.Increment(ref _listObjectVersionsCalls);
            ListObjectVersionsRequest request = new ListObjectVersionsRequest(bucketName);
            config?.Invoke(request);

            if (call == 2)
            {
                SecondVersionRequestKeyMarker = request.KeyMarker;
                SecondVersionRequestVersionIdMarker = request.VersionIdMarker;
            }

            ListObjectVersionsResponse response = new ListObjectVersionsResponse
            {
                IsSuccess = true,
                IsTruncated = call == 1,
                NextKeyMarker = "next-key",
                NextVersionIdMarker = "next-version"
            };

            response.Versions.Add(new S3Version("version-" + call, "id-" + call, true, DateTimeOffset.UnixEpoch, "etag", 0, null, default, default, default));
            return Task.FromResult(response);
        }
    }

    private sealed class PagingMultipartClient(bool failFirstPage = false) : IMultipartClient
    {
        private int _calls;

        public string? SecondRequestKeyMarker { get; private set; }
        public string? SecondRequestUploadIdMarker { get; private set; }

        public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest>? config = null, CancellationToken token = default)
        {
            int call = Interlocked.Increment(ref _calls);
            ListMultipartUploadsRequest request = new ListMultipartUploadsRequest(bucketName);
            config?.Invoke(request);

            if (call == 2)
            {
                SecondRequestKeyMarker = request.KeyMarker;
                SecondRequestUploadIdMarker = request.UploadIdMarker;
            }

            ListMultipartUploadsResponse response = new ListMultipartUploadsResponse
            {
                IsSuccess = !failFirstPage,
                IsTruncated = call == 1,
                NextKeyMarker = "next-key",
                NextUploadIdMarker = "next-upload"
            };

            response.Uploads.Add(new S3Upload("upload-" + call, "upload-id-" + call, null, null, default, DateTimeOffset.UnixEpoch));
            return Task.FromResult(response);
        }

        public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<UploadPartCopyResponse> UploadPartCopyAsync(string sourceBucketName, string sourceObjectKey, string destinationBucketName, string destinationObjectKey, int partNumber, string uploadId, Action<UploadPartCopyRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<S3PartInfo> parts, Action<CompleteMultipartUploadRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
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
        public Task<UploadPartCopyResponse> UploadPartCopyAsync(string sourceBucketName, string sourceObjectKey, string destinationBucketName, string destinationObjectKey, int partNumber, string uploadId, Action<UploadPartCopyRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
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
        public Task<GetBucketAclResponse> GetBucketAclAsync(string bucketName, Action<GetBucketAclRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutBucketAclResponse> PutBucketAclAsync(string bucketName, Action<PutBucketAclRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetBucketCorsResponse> GetBucketCorsAsync(string bucketName, Action<GetBucketCorsRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutBucketCorsResponse> PutBucketCorsAsync(string bucketName, IEnumerable<S3CorsRule> rules, Action<PutBucketCorsRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeleteBucketCorsResponse> DeleteBucketCorsAsync(string bucketName, Action<DeleteBucketCorsRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetBucketOwnershipControlsResponse> GetBucketOwnershipControlsAsync(string bucketName, Action<GetBucketOwnershipControlsRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutBucketOwnershipControlsResponse> PutBucketOwnershipControlsAsync(string bucketName, IEnumerable<S3OwnershipControlsRule> rules, Action<PutBucketOwnershipControlsRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeleteBucketOwnershipControlsResponse> DeleteBucketOwnershipControlsAsync(string bucketName, Action<DeleteBucketOwnershipControlsRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetBucketLocationResponse> GetBucketLocationAsync(string bucketName, Action<GetBucketLocationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetBucketRequestPaymentResponse> GetBucketRequestPaymentAsync(string bucketName, Action<GetBucketRequestPaymentRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutBucketRequestPaymentResponse> PutBucketRequestPaymentAsync(string bucketName, RequestPaymentPayer payer, Action<PutBucketRequestPaymentRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetBucketPolicyStatusResponse> GetBucketPolicyStatusAsync(string bucketName, Action<GetBucketPolicyStatusRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetBucketWebsiteResponse> GetBucketWebsiteAsync(string bucketName, Action<GetBucketWebsiteRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutBucketWebsiteResponse> PutBucketWebsiteAsync(string bucketName, S3WebsiteConfiguration websiteConfiguration, Action<PutBucketWebsiteRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeleteBucketWebsiteResponse> DeleteBucketWebsiteAsync(string bucketName, Action<DeleteBucketWebsiteRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeleteBucketAnalyticsConfigurationResponse> DeleteBucketAnalyticsConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketAnalyticsConfigurationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeleteBucketInventoryConfigurationResponse> DeleteBucketInventoryConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketInventoryConfigurationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeleteBucketMetricsConfigurationResponse> DeleteBucketMetricsConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketMetricsConfigurationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeleteBucketIntelligentTieringConfigurationResponse> DeleteBucketIntelligentTieringConfigurationAsync(string bucketName, string configurationId, Action<DeleteBucketIntelligentTieringConfigurationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeleteBucketReplicationResponse> DeleteBucketReplicationAsync(string bucketName, Action<DeleteBucketReplicationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<UpdateBucketMetadataAnnotationTableConfigurationResponse> UpdateBucketMetadataAnnotationTableConfigurationAsync(string bucketName, S3AnnotationTableConfiguration configuration, Action<UpdateBucketMetadataAnnotationTableConfigurationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutBucketTaggingResponse> PutBucketTaggingAsync(string bucketName, IDictionary<string, string> tags, Action<PutBucketTaggingRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeleteBucketTaggingResponse> DeleteBucketTaggingAsync(string bucketName, Action<DeleteBucketTaggingRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutBucketAccelerateConfigurationResponse> PutBucketAccelerateConfigurationAsync(string bucketName, bool enabled, Action<PutBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetBucketAccelerateConfigurationResponse> GetBucketAccelerateConfigurationAsync(string bucketName, Action<GetBucketAccelerateConfigurationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutBucketLifecycleConfigurationResponse> PutBucketLifecycleConfigurationAsync(string bucketName, IEnumerable<S3Rule> rules, Action<PutBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeleteBucketLifecycleResponse> DeleteBucketLifecycleAsync(string bucketName, Action<DeleteBucketLifecycleRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutBucketEncryptionResponse> PutBucketEncryptionAsync(string bucketName, IEnumerable<S3ServerSideEncryptionRule> rules, Action<PutBucketEncryptionRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetBucketEncryptionResponse> GetBucketEncryptionAsync(string bucketName, Action<GetBucketEncryptionRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeleteBucketEncryptionResponse> DeleteBucketEncryptionAsync(string bucketName, Action<DeleteBucketEncryptionRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutBucketVersioningResponse> PutBucketVersioningAsync(string bucketName, bool enabled, Action<PutBucketVersioningRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetBucketVersioningResponse> GetBucketVersioningAsync(string bucketName, Action<GetBucketVersioningRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetBucketLifecycleConfigurationResponse> GetBucketLifecycleConfigurationAsync(string bucketName, Action<GetBucketLifecycleConfigurationRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutPublicAccessBlockResponse> PutPublicAccessBlockAsync(string bucketName, Action<PutPublicAccessBlockRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetPublicAccessBlockResponse> GetPublicAccessBlockAsync(string bucketName, Action<GetPublicAccessBlockRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeletePublicAccessBlockResponse> DeletePublicAccessBlockAsync(string bucketName, Action<DeletePublicAccessBlockRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetBucketPolicyResponse> GetBucketPolicyAsync(string bucketName, Action<GetBucketPolicyRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeleteBucketPolicyResponse> DeleteBucketPolicyAsync(string bucketName, Action<DeleteBucketPolicyRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutBucketPolicyResponse> PutBucketPolicyAsync(string bucketName, string policy, Action<PutBucketPolicyRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetBucketAbacResponse> GetBucketAbacAsync(string bucketName, Action<GetBucketAbacRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutBucketAbacResponse> PutBucketAbacAsync(string bucketName, bool enabled, Action<PutBucketAbacRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
    }
}