using System.Collections.Generic;
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

        /// <summary>Deletes an object See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_DeleteObject.html for details</summary>
        Task<DeleteObjectResponse> DeleteObjectAsync(DeleteObjectRequest request, CancellationToken token = default);

        /// <summary>Check if an object exists See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_HeadObject.html for details</summary>
        Task<HeadObjectResponse> HeadObjectAsync(HeadObjectRequest request, CancellationToken token = default);

        /// <summary>Create a multipart upload See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_CreateMultipartUpload.html for details</summary>
        Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(CreateMultipartUploadRequest request, CancellationToken token = default);

        /// <summary>Upload a part to a multipart upload See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_UploadPart.html for details</summary>
        Task<UploadPartResponse> UploadPartAsync(UploadPartRequest request, CancellationToken token = default);

        /// <summary>List all parts as part of a multipart upload See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_ListParts.html for details</summary>
        Task<ListPartsResponse> ListPartsAsync(ListPartsRequest request, CancellationToken token = default);

        /// <summary>Complete a multipart upload See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_CompleteMultipartUpload.html for details</summary>
        Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(CompleteMultipartUploadRequest request, CancellationToken token = default);

        /// <summary>Abort a multipart upload See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_AbortMultipartUpload.html for details</summary>
        Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(AbortMultipartUploadRequest request, CancellationToken token = default);

        /// <summary>Delete multiple objects in one request See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_DeleteObjects.html for details</summary>
        Task<DeleteObjectsResponse> DeleteObjectsAsync(DeleteObjectsRequest request, CancellationToken token = default);

        /// <summary>Put an object into the S3 bucket See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_PutObject.html for details</summary>
        Task<PutObjectResponse> PutObjectAsync(PutObjectRequest request, CancellationToken token = default);

        /// <summary>Get an object from an S3 bucket See https://docs.aws.amazon.com/en_pv/AmazonS3/latest/API/API_GetObject.html for details</summary>
        Task<GetObjectResponse> GetObjectAsync(GetObjectRequest request, CancellationToken token = default);
    }
}