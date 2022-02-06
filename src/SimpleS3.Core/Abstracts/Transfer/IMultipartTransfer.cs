#if COMMERCIAL
using System.Runtime.CompilerServices;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
#endif

namespace Genbox.SimpleS3.Core.Abstracts.Transfer;

public interface IMultipartTransfer
{
#if COMMERCIAL

    /// <summary>An extension that performs multipart download. It only works if the file that gets downloaded was originally
    /// uploaded using multipart, otherwise it falls back to an ordinary get request. Note that the implementation is designed
    /// to avoid excessive memory usage, so it seeks in the output stream whenever data is available.</summary>
    IAsyncEnumerable<GetObjectResponse> MultipartDownloadAsync(string bucketName, string objectKey, Stream output, int bufferSize = 16777216, int numParallelParts = 4, Action<GetObjectRequest>? config = null, CancellationToken token = default);

    Task<CompleteMultipartUploadResponse> MultipartUploadAsync(string bucketName, string objectKey, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<CreateMultipartUploadRequest>? config = null, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default);

    /// <summary>An extension that performs multipart upload.</summary>
    Task<CompleteMultipartUploadResponse> MultipartUploadAsync(CreateMultipartUploadRequest req, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default);
#endif
}