using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Clients
{
    [PublicAPI]
    public interface IMultipartClient
    {
        IMultipartOperations MultipartOperations { get; }

        /// <summary>
        /// Create a multipart upload. Once created, you can start uploading parts to it. Remember to call either
        /// <see cref="CompleteMultipartUploadAsync" /> or <see cref="AbortMultipartUploadAsync" /> when you are finished.
        /// </summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest>? config = null, CancellationToken token = default);

        /// <summary>Upload a part to a multipart upload. You must call <see cref="CreateMultipartUploadAsync" /> before you can upload parts to it.</summary>
        /// <param name="partNumber">The index of the part</param>
        /// <param name="uploadId">The upload id of the multipart upload</param>
        /// <param name="content">Content of the part</param>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest>? config = null, CancellationToken token = default);

        /// <summary>List the parts of a multipart upload</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="uploadId">The upload id of the multipart upload</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest>? config = null, CancellationToken token = default);

        /// <summary>Mark a multipart upload as completed</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="uploadId">The upload id of the multipart upload</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<UploadPartResponse> parts, Action<CompleteMultipartUploadRequest>? config = null, CancellationToken token = default);

        /// <summary>Abort a multipart upload. You have to call this if you want to abort, otherwise you will pay for the unfinished multipart</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="uploadId">The upload id of the multipart upload</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest>? config = null, CancellationToken token = default);

        /// <summary>List all multipart uploads within a bucket</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<ListMultipartUploadsResponse> ListMultipartUploadsAsync(string bucketName, Action<ListMultipartUploadsRequest>? config = null, CancellationToken token = default);
    }
}