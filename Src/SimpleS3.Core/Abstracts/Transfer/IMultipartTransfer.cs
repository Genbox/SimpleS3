using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Abstracts.Transfer;

/// <summary>Provides the <c>IMultipartTransfer</c> API.</summary>
public interface IMultipartTransfer
{
    /// <summary>
    /// An extension that performs multipart download. It only works if the file that gets downloaded was originally uploaded using multipart, otherwise it falls back to an
    /// <remarks>
    /// Downloads start at the output stream's current position and preserve its prefix. Requests use the initially selected
    /// version, or an ETag condition when no immutable version is available. The helper controls part selection; configuring
    /// Range or PartNumber is not supported. Returned responses report completed parts; their content has already been disposed.
    /// A failed or canceled download can leave partial data in the destination.
    /// </remarks>
    /// ordinary get request. Note that the implementation is designed to avoid excessive memory usage, so it seeks in the output stream whenever data is available.
    /// </summary>
    IAsyncEnumerable<GetObjectResponse> MultipartDownloadAsync(string bucketName, string objectKey, Stream output, int bufferSize = 16777216, int numParallelParts = 4, Action<GetObjectRequest>? config = null, CancellationToken token = default);

    /// <summary>Provides the <c>MultipartUploadAsync</c> API.</summary>
    Task<CompleteMultipartUploadResponse> MultipartUploadAsync(string bucketName, string objectKey, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<CreateMultipartUploadRequest>? config = null, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default);

    /// <summary>An extension that performs multipart upload.</summary>
    Task<CompleteMultipartUploadResponse> MultipartUploadAsync(CreateMultipartUploadRequest req, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default);
}