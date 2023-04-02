using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Operations;

[PublicAPI]
public interface IMultipartOperations
{
    IList<IRequestWrapper> RequestWrappers { get; }
    IList<IResponseWrapper> ResponseWrappers { get; }

    /// <summary>Create a multipart upload See
    /// https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_CreateMultipartUpload.html for details</summary>
    Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(CreateMultipartUploadRequest request, CancellationToken token = default);

    /// <summary>Upload a part to a multipart upload See
    /// https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_UploadPart.html for details</summary>
    Task<UploadPartResponse> UploadPartAsync(UploadPartRequest request, CancellationToken token = default);

    /// <summary>List all parts as part of a multipart upload See
    /// https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_ListParts.html for details</summary>
    Task<ListPartsResponse> ListPartsAsync(ListPartsRequest request, CancellationToken token = default);

    /// <summary>Complete a multipart upload See
    /// https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_CompleteMultipartUpload.html for details</summary>
    Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(CompleteMultipartUploadRequest request, CancellationToken token = default);

    /// <summary>Abort a multipart upload See
    /// https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_AbortMultipartUpload.html for details</summary>
    Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(AbortMultipartUploadRequest request, CancellationToken token = default);

    /// <summary>List in-progress multipart uploads See
    /// https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_ListMultipartUploads.html for details</summary>
    Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(ListMultipartUploadsRequest request, CancellationToken token = default);
}