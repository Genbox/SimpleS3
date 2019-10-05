using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Requests.Objects;
using Genbox.SimpleS3.Core.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Operations
{
    [PublicAPI]
    public interface IObjectOperations
    {
        IList<IRequestWrapper> RequestWrappers { get; }
        IList<IResponseWrapper> ResponseWrappers { get; }
        Task<DeleteObjectResponse> DeleteAsync(DeleteObjectRequest request, CancellationToken token = default);
        Task<HeadObjectResponse> HeadAsync(HeadObjectRequest request, CancellationToken token = default);
        Task<InitiateMultipartUploadResponse> InitiateMultipartUploadAsync(InitiateMultipartUploadRequest request, CancellationToken token = default);
        Task<UploadPartResponse> UploadPartAsync(UploadPartRequest request, CancellationToken token = default);
        Task<ListPartsResponse> ListPartsAsync(ListPartsRequest request, CancellationToken token = default);
        Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(CompleteMultipartUploadRequest request, CancellationToken token = default);
        Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(AbortMultipartUploadRequest request, CancellationToken token = default);
        Task<DeleteMultipleObjectsResponse> DeleteMultipleAsync(DeleteMultipleObjectsRequest request, CancellationToken token = default);
        Task<PutObjectResponse> PutAsync(PutObjectRequest request, CancellationToken token = default);
        Task<GetObjectResponse> GetAsync(GetObjectRequest request, CancellationToken token = default);
        IAsyncEnumerable<UploadPartResponse> MultipartUploadAsync(InitiateMultipartUploadRequest req, Stream data, int partSize = 16777216, int numParallelParts = 4, CancellationToken token = default);
        IAsyncEnumerable<GetObjectResponse> MultipartDownloadAsync(string bucketName, string resource, Stream output, int bufferSize = 16777216, int numParallelParts = 4, Action<GetObjectRequest> config = null, CancellationToken token = default);
    }
}