using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Requests.Objects;
using Genbox.SimpleS3.Core.Requests.Objects.Types;
using Genbox.SimpleS3.Core.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Clients
{
    [PublicAPI]
    public interface IS3ObjectClient
    {
        Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string resource, Action<DeleteObjectRequest> config = null, CancellationToken token = default);
        Task<DeleteMultipleObjectsResponse> DeleteMultipleObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> resources, Action<DeleteMultipleObjectsRequest> config = null, CancellationToken token = default);
        Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string resource, Action<HeadObjectRequest> config = null, CancellationToken token = default);
        Task<InitiateMultipartUploadResponse> InitiateMultipartUploadAsync(string bucketName, string resource, Action<InitiateMultipartUploadRequest> config = null, CancellationToken token = default);
        Task<UploadPartResponse> UploadPartAsync(string bucketName, string resource, int partNumber, string uploadId, Stream content, Action<UploadPartRequest> config = null, CancellationToken token = default);
        Task<ListPartsResponse> ListPartsAsync(string bucketName, string resource, string uploadId, Action<ListPartsRequest> config = null, CancellationToken token = default);
        Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string resource, string uploadId, IEnumerable<UploadPartResponse> parts, Action<CompleteMultipartUploadRequest> config = null, CancellationToken token = default);
        Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string resource, string uploadId, Action<AbortMultipartUploadRequest> config = null, CancellationToken token = default);
        Task<GetObjectResponse> GetObjectAsync(string bucketName, string resource, Action<GetObjectRequest> config = null, CancellationToken token = default);
        Task<PutObjectResponse> PutObjectAsync(string bucketName, string resource, Stream data, Action<PutObjectRequest> config = null, CancellationToken token = default);
        Task<MultipartUploadStatus> MultipartUploadAsync(string bucketName, string resource, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<InitiateMultipartUploadRequest> config = null, CancellationToken token = default);
        Task<MultipartDownloadStatus> MultipartDownloadAsync(string bucketName, string resource, Stream output, int bufferSize = 16777216, int numParallelParts = 4, CancellationToken token = default);
    }
}