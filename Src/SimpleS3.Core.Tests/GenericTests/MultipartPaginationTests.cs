using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class MultipartPaginationTests
{
    [Fact]
    public async Task ListAllUploadsReusesDecodedKeyMarker()
    {
        RecordingClient client = new RecordingClient();

        int uploads = 0;
        await foreach (S3Upload _ in client.ListAllMultipartUploadsAsync("bucket", TestContext.Current.CancellationToken))
            uploads++;

        Assert.Equal(0, uploads);
        Assert.Equal("literal%2F", client.SecondKeyMarker);
        Assert.Equal("upload-id", client.SecondUploadIdMarker);
    }

    private sealed class RecordingClient : IMultipartClient
    {
        private int _pages;

        public string? SecondKeyMarker { get; private set; }
        public string? SecondUploadIdMarker { get; private set; }

        public Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest>? config = null, CancellationToken token = default)
        {
            ListMultipartUploadsRequest request = new ListMultipartUploadsRequest(bucketName);
            config?.Invoke(request);

            if (_pages++ == 0)
            {
                return Task.FromResult(new ListMultipartUploadsResponse
                {
                    IsSuccess = true,
                    IsTruncated = true,
                    EncodingType = EncodingType.Url,
                    NextKeyMarker = "literal%2F",
                    NextUploadIdMarker = "upload-id"
                });
            }

            SecondKeyMarker = request.KeyMarker;
            SecondUploadIdMarker = request.UploadIdMarker;
            return Task.FromResult(new ListMultipartUploadsResponse { IsSuccess = true });
        }

        public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<UploadPartCopyResponse> UploadPartCopyAsync(string sourceBucketName, string sourceObjectKey, string destinationBucketName, string destinationObjectKey, int partNumber, string uploadId, Action<UploadPartCopyRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<S3PartInfo> parts, Action<CompleteMultipartUploadRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
        public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest>? config = null, CancellationToken token = default) => throw new NotSupportedException();
    }
}