using System.Globalization;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Internals.Fluent;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class MultipartTransferTests
{
    [Fact]
    public async Task MultipartDownloadAsyncStopsSchedulingPartsWhenPartFails()
    {
        FailingObjectClient objectClient = new FailingObjectClient();
        MultipartTransfer transfer = new MultipartTransfer(objectClient, null!, null!, []);

        await using MemoryStream output = new MemoryStream();

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await foreach (GetObjectResponse _ in transfer.MultipartDownloadAsync("bucket", "object", output, bufferSize: 1, numParallelParts: 2, token: TestContext.Current.CancellationToken)) {}
        });

        Assert.True(objectClient.GetPartRequests < 10);
    }

    [Fact]
    public async Task MultipartUploadAsyncSerializesPartResponseCallbacks()
    {
        FakeMultipartApi multipartApi = new FakeMultipartApi();
        MultipartTransfer transfer = new MultipartTransfer(null!, multipartApi, multipartApi, []);
        await using NonSeekableDataStream data = new NonSeekableDataStream([1, 2, 3, 4]);
        int activeCallbacks = 0;
        int maxActiveCallbacks = 0;

        CompleteMultipartUploadResponse response = await transfer.MultipartUploadAsync("bucket", "object", data, partSize: 1, numParallelParts: 4, onPartResponse: _ =>
        {
            int active = Interlocked.Increment(ref activeCallbacks);
            SetMax(ref maxActiveCallbacks, active);

            Thread.Sleep(20);
            Interlocked.Decrement(ref activeCallbacks);
        }, token: TestContext.Current.CancellationToken).ConfigureAwait(false);

        Assert.True(response.IsSuccess);
        Assert.Equal(1, Volatile.Read(ref maxActiveCallbacks));
    }

    [Fact]
    public async Task MultipartUploadAsyncPassesCancellationTokenToChunkReads()
    {
        FakeMultipartApi multipartApi = new FakeMultipartApi();
        MultipartTransfer transfer = new MultipartTransfer(null!, multipartApi, multipartApi, []);
        await using BlockingReadStream data = new BlockingReadStream();
        using CancellationTokenSource cts = new CancellationTokenSource();

        Task<CompleteMultipartUploadResponse> uploadTask = transfer.MultipartUploadAsync("bucket", "object", data, partSize: 1, numParallelParts: 1, token: cts.Token);

        await data.ReadStarted.Task.WaitAsync(TestContext.Current.CancellationToken).ConfigureAwait(false);
        await cts.CancelAsync().ConfigureAwait(false);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => uploadTask);
        Assert.True(data.CapturedToken.IsCancellationRequested);
    }

    private static void SetMax(ref int target, int value)
    {
        while (true)
        {
            int current = Volatile.Read(ref target);

            if (current >= value)
                return;

            if (Interlocked.CompareExchange(ref target, value, current) == current)
                return;
        }
    }

    private sealed class FailingObjectClient : IObjectClient
    {
        private int _getPartRequests;

        public int GetPartRequests => Volatile.Read(ref _getPartRequests);

        public Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest>? config = null, CancellationToken token = default)
        {
            HeadObjectRequest request = new HeadObjectRequest(bucketName, objectKey);
            config?.Invoke(request);

            return Task.FromResult(new HeadObjectResponse
            {
                ContentLength = 1,
                IsSuccess = true,
                NumberOfParts = request.PartNumber == 1 ? 10 : null
            });
        }

        public async Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest>? config = null, CancellationToken token = default)
        {
            GetObjectRequest request = new GetObjectRequest(bucketName, objectKey);
            config?.Invoke(request);
            Interlocked.Increment(ref _getPartRequests);

            if (request.PartNumber == 1)
                throw new InvalidOperationException();

            await Task.Delay(Timeout.InfiniteTimeSpan, token).ConfigureAwait(false);
            return new GetObjectResponse();
        }

        public Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream? data, Action<PutObjectRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<ListObjectsResponse> ListObjectsAsync(string bucketName, Action<ListObjectsRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<RestoreObjectResponse> RestoreObjectAsync(string bucketName, string objectKey, Action<RestoreObjectRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<CopyObjectResponse> CopyObjectAsync(string sourceBucketName, string sourceObjectKey, string destinationBucket, string destinationObjectKey, Action<CopyObjectRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutObjectAclResponse> PutObjectAclAsync(string bucketName, string objectKey, Action<PutObjectAclRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetObjectAclResponse> GetObjectAclAsync(string bucketName, string objectKey, Action<GetObjectAclRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<GetObjectLegalHoldResponse> GetObjectLegalHoldAsync(string bucketName, string objectKey, Action<GetObjectLegalHoldRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<PutObjectLegalHoldResponse> PutObjectLegalHoldAsync(string bucketName, string objectKey, bool lockStatus, Action<PutObjectLegalHoldRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<ListObjectVersionsResponse> ListObjectVersionsAsync(string bucketName, Action<ListObjectVersionsRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
    }

    private sealed class FakeMultipartApi : IMultipartClient, IMultipartOperations
    {
        public IList<IRequestWrapper> RequestWrappers { get; } = Array.Empty<IRequestWrapper>();
        public IList<IResponseWrapper> ResponseWrappers { get; } = Array.Empty<IResponseWrapper>();

        public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(CreateMultipartUploadRequest request, CancellationToken token = default)
        {
            return Task.FromResult(new CreateMultipartUploadResponse
            {
                IsSuccess = true,
                UploadId = "upload-id"
            });
        }

        public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();

        public async Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest>? config = null, CancellationToken token = default)
        {
            await Task.Delay(10, token).ConfigureAwait(false);

            return new UploadPartResponse
            {
                ETag = "etag-" + partNumber.ToString(NumberFormatInfo.InvariantInfo),
                IsSuccess = true,
                PartNumber = partNumber
            };
        }

        public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<S3PartInfo> parts, Action<CompleteMultipartUploadRequest>? config = null, CancellationToken token = default)
        {
            return Task.FromResult(new CompleteMultipartUploadResponse
            {
                IsSuccess = true
            });
        }

        public Task<UploadPartResponse> UploadPartAsync(UploadPartRequest request, CancellationToken token = default) => throw new NotSupportedException();
        public Task<ListPartsResponse> ListPartsAsync(ListPartsRequest request, CancellationToken token = default) => throw new NotSupportedException();
        public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(CompleteMultipartUploadRequest request, CancellationToken token = default) => throw new NotSupportedException();
        public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(AbortMultipartUploadRequest request, CancellationToken token = default) => throw new NotSupportedException();
        public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(ListMultipartUploadsRequest request, CancellationToken token = default) => throw new NotSupportedException();
        public Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();

        public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest>? config = null, CancellationToken token = default)
        {
            return Task.FromResult(new AbortMultipartUploadResponse
            {
                IsSuccess = true
            });
        }

        public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
    }

    private sealed class NonSeekableDataStream(byte[] data) : Stream
    {
        private readonly MemoryStream _inner = new MemoryStream(data);

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _inner.Length;

        public override long Position
        {
            get => _inner.Position;
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void Flush() {}
    }

    private sealed class BlockingReadStream : Stream
    {
        public TaskCompletionSource ReadStarted { get; } = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        public CancellationToken CapturedToken { get; private set; }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            CapturedToken = cancellationToken;
            ReadStarted.TrySetResult();
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken).ConfigureAwait(false);
            return 0;
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            CapturedToken = cancellationToken;
            ReadStarted.TrySetResult();
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken).ConfigureAwait(false);
            return 0;
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void Flush() {}
    }
}