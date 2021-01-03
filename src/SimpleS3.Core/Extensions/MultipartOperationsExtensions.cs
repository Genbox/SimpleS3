using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;

namespace Genbox.SimpleS3.Core.Extensions
{
    public static class MultipartOperationsExtensions
    {
        public static Task<CompleteMultipartUploadResponse> MultipartUploadAsync(this IMultipartOperations operation, string bucketName, string objectKey, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<CreateMultipartUploadRequest>? config = null, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default)
        {
            CreateMultipartUploadRequest req = new CreateMultipartUploadRequest(bucketName, objectKey);
            config?.Invoke(req);

            return MultipartUploadAsync(operation, req, data, partSize, numParallelParts, onPartResponse, token);
        }

        /// <summary>An extension that performs multipart upload.</summary>
        public static async Task<CompleteMultipartUploadResponse> MultipartUploadAsync(this IMultipartOperations operations, CreateMultipartUploadRequest req, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default)
        {
            Validator.RequireNotNull(req, nameof(req));
            Validator.RequireNotNull(data, nameof(data));

            foreach (IRequestWrapper wrapper in operations.RequestWrappers)
            {
                if (wrapper.IsSupported(req))
                    data = wrapper.Wrap(data, req);
            }

            string bucket = req.BucketName;
            string objectKey = req.ObjectKey;

            byte[]? encryptionKey = null;

            try
            {
                if (req.SseCustomerKey != null)
                {
                    encryptionKey = new byte[req.SseCustomerKey.Length];
                    Array.Copy(req.SseCustomerKey, 0, encryptionKey, 0, encryptionKey.Length);
                }

                CreateMultipartUploadResponse initResp = await operations.CreateMultipartUploadAsync(req, token).ConfigureAwait(false);

                if (token.IsCancellationRequested)
                    return new CompleteMultipartUploadResponse { BucketName = bucket, ObjectKey = objectKey };

                if (!initResp.IsSuccess)
                    throw new S3RequestException(initResp.StatusCode, "CreateMultipartUploadRequest was unsuccessful");

                IEnumerable<ArraySegment<byte>> chunks = ReadChunks(data, partSize);

                int partNumber = 0;

                IEnumerable<UploadPartResponse> responses = await ParallelHelper.ExecuteAsync(chunks, async bytes =>
                {
                    Interlocked.Increment(ref partNumber);

                    using (MemoryStream ms = new MemoryStream(bytes.Array!, 0, bytes.Count))
                    {
                        UploadPartRequest uploadReq = new UploadPartRequest(bucket, objectKey, partNumber, initResp.UploadId, ms);
                        uploadReq.SseCustomerAlgorithm = req.SseCustomerAlgorithm;
                        uploadReq.SseCustomerKey = encryptionKey;
                        uploadReq.SseCustomerKeyMd5 = req.SseCustomerKeyMd5;

                        UploadPartResponse resp = await operations.UploadPartAsync(uploadReq, token).ConfigureAwait(false);
                        onPartResponse?.Invoke(resp);
                        return resp;
                    }

                }, numParallelParts, token);

                CompleteMultipartUploadRequest completeReq = new CompleteMultipartUploadRequest(bucket, objectKey, initResp.UploadId, responses.OrderBy(x => x.PartNumber));
                CompleteMultipartUploadResponse completeResp = await operations.CompleteMultipartUploadAsync(completeReq, token).ConfigureAwait(false);

                return completeResp;
            }
            finally
            {
                if (encryptionKey != null)
                    Array.Clear(encryptionKey, 0, encryptionKey.Length);
            }
        }

        private static IEnumerable<ArraySegment<byte>> ReadChunks(Stream data, int chunkSize)
        {
            while (true)
            {
                byte[] chunkData = new byte[chunkSize];
                int read = data.ReadUpTo(chunkData, 0, chunkData.Length);

                if (read == 0)
                    break;

                yield return new ArraySegment<byte>(chunkData, 0, read);
            }
        }
    }
}