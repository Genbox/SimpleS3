using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Operations;
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
        IObjectOperations ObjectOperations { get; }

        /// <summary>Delete an object</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<DeleteObjectResponse> DeleteObjectAsync(string bucketName, string objectKey, Action<DeleteObjectRequest> config = null, CancellationToken token = default);


        /// <summary>Delete multiple objects</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKeys">A list of object keys</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<DeleteObjectsResponse> DeleteObjectsAsync(string bucketName, IEnumerable<S3DeleteInfo> objectKeys, Action<DeleteObjectsRequest> config = null, CancellationToken token = default);

        /// <summary>Head an object. Can be used to check if an object exists without downloading the content of it.</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<HeadObjectResponse> HeadObjectAsync(string bucketName, string objectKey, Action<HeadObjectRequest> config = null, CancellationToken token = default);

        /// <summary>
        /// Create a multipart upload. Once created, you can start uploading parts to it. Remember to call either
        /// <see cref="CompleteMultipartUploadAsync" /> or <see cref="AbortMultipartUploadAsync" /> when you are finished.
        /// </summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(string bucketName, string objectKey, Action<CreateMultipartUploadRequest> config = null, CancellationToken token = default);

        /// <summary>Upload a part to a multipart upload. You must call <see cref="CreateMultipartUploadAsync" /> before you can upload parts to it.</summary>
        /// <param name="partNumber">The index of the part</param>
        /// <param name="uploadId">The upload id of the multipart upload</param>
        /// <param name="content">Content of the part</param>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<UploadPartResponse> UploadPartAsync(string bucketName, string objectKey, int partNumber, string uploadId, Stream content, Action<UploadPartRequest> config = null, CancellationToken token = default);

        /// <summary>List the parts of a multipart upload</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="uploadId">The upload id of the multipart upload</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<ListPartsResponse> ListPartsAsync(string bucketName, string objectKey, string uploadId, Action<ListPartsRequest> config = null, CancellationToken token = default);

        /// <summary>Mark a multipart upload as completed</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="uploadId">The upload id of the multipart upload</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(string bucketName, string objectKey, string uploadId, IEnumerable<UploadPartResponse> parts, Action<CompleteMultipartUploadRequest> config = null, CancellationToken token = default);

        /// <summary>Abort a multipart upload. You have to call this if you want to abort, otherwise you will pay for the unfinished multipart</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="uploadId">The upload id of the multipart upload</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(string bucketName, string objectKey, string uploadId, Action<AbortMultipartUploadRequest> config = null, CancellationToken token = default);

        /// <summary>Get (download) an object</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<GetObjectResponse> GetObjectAsync(string bucketName, string objectKey, Action<GetObjectRequest> config = null, CancellationToken token = default);

        /// <summary>Put (upload) an object</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="data">The content of the object</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        Task<PutObjectResponse> PutObjectAsync(string bucketName, string objectKey, Stream data, Action<PutObjectRequest> config = null, CancellationToken token = default);

        /// <summary>A convenience method to automate a multipart upload</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="data">The content of the object</param>
        /// <param name="numParallelParts">The number of parallel uploads</param>
        /// <param name="config">A delegate to configure the request</param>
        /// <param name="token">A cancellation token</param>
        /// <param name="partSize">Size of each part</param>
        Task<MultipartUploadStatus> MultipartUploadAsync(string bucketName, string objectKey, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<CreateMultipartUploadRequest> config = null, CancellationToken token = default);

        /// <summary>A convenience method to automate a multipart download</summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="objectKey">The key of the object</param>
        /// <param name="bufferSize">The number of bytes to buffer before flushing to <see cref="output" /></param>
        /// <param name="numParallelParts">The number of parallel uploads</param>
        /// <param name="token">A cancellation token</param>
        /// <param name="output">The stream you want to download to</param>
        Task<MultipartDownloadStatus> MultipartDownloadAsync(string bucketName, string objectKey, Stream output, int bufferSize = 16777216, int numParallelParts = 4, CancellationToken token = default);
    }
}