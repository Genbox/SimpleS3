using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Extensions
{
    public static class MultipartOperationsExtensions
    {
        /// <summary>An extension that performs multipart upload.</summary>
        public static async Task<CompleteMultipartUploadResponse?> MultipartUploadAsync(this IMultipartOperations operations, CreateMultipartUploadRequest req, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default)
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
                    return null;

                if (!initResp.IsSuccess)
                    throw new S3RequestException(initResp.StatusCode, "CreateMultipartUploadRequest was unsuccessful");

                IAsyncEnumerable<byte[]> chunks = ReadChunksAsync(data, partSize, token);

                int partNumber = 0;

                IEnumerable<UploadPartResponse> responses = await ParallelHelper.ExecuteAsync(chunks, async bytes =>
                 {
                     Interlocked.Increment(ref partNumber);

                     using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
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

        private static async IAsyncEnumerable<byte[]> ReadChunksAsync(Stream data, int chunkSize, [EnumeratorCancellation] CancellationToken token)
        {
            while (true)
            {
                byte[] chunkData = new byte[chunkSize];
                int read = await data.ReadUpToAsync(chunkData, 0, chunkData.Length, token).ConfigureAwait(false);

                if (read == 0)
                    break;

                yield return chunkData;
            }
        }

        /// <summary>
        /// An extension that performs multipart download. It only works if the file that gets downloaded was originally uploaded using multipart,
        /// otherwise it falls back to an ordinary get request. Note that the implementation is designed to avoid excessive memory usage, so it seeks in the
        /// output stream whenever data is available.
        /// </summary>
        public static async IAsyncEnumerable<GetObjectResponse> MultipartDownloadAsync(this IObjectOperations operations, string bucketName, string objectKey, Stream output, int bufferSize = 16777216, int numParallelParts = 4, Action<GetObjectRequest>? config = null, [EnumeratorCancellation] CancellationToken token = default)
        {
            Validator.RequireNotNull(output, nameof(output));

            //Use a HEAD request on the object key to determine if the file was originally uploaded with multipart
            HeadObjectRequest headReq = new HeadObjectRequest(bucketName, objectKey);
            headReq.PartNumber = 1;

            HeadObjectResponse headResp = await operations.HeadObjectAsync(headReq, token).ConfigureAwait(false);

            Queue<Task<GetObjectResponse>> queue = new Queue<Task<GetObjectResponse>>();

            if (headResp.NumberOfParts == null)
            {
                GetObjectRequest getReq = new GetObjectRequest(bucketName, objectKey);
                config?.Invoke(getReq);

                GetObjectResponse getResp = await operations.GetObjectAsync(getReq, token).ConfigureAwait(false);

                if (!getResp.IsSuccess)
                    throw new S3RequestException(getResp.StatusCode);

                await getResp.Content.CopyToAsync(output, 81920, token).ConfigureAwait(false);

                yield return getResp;
            }
            else
            {
                int parts = headResp.NumberOfParts.Value;

                using (SemaphoreSlim semaphore = new SemaphoreSlim(numParallelParts))
                using (Mutex mutex = new Mutex())
                {
                    for (int i = 1; i <= parts; i++)
                    {
                        await semaphore.WaitAsync(token).ConfigureAwait(false);

                        if (token.IsCancellationRequested)
                            yield break;

                        queue.Enqueue(DownloadPartAsync(operations, bucketName, objectKey, output, headResp.ContentLength, i, bufferSize, semaphore, mutex, config, token));
                    }

                    while (queue.TryDequeue(out Task<GetObjectResponse>? task))
                    {
                        if (token.IsCancellationRequested)
                            yield break;

                        GetObjectResponse response = await task!.ConfigureAwait(false);
                        yield return response;
                    }
                }
            }
        }

        private static async Task<GetObjectResponse> DownloadPartAsync(IObjectOperations operations, string bucketName, string objectKey, Stream output, long partSize, int partNumber, int bufferSize, SemaphoreSlim semaphore, Mutex mutex, Action<GetObjectRequest>? config, CancellationToken token)
        {
            try
            {
                GetObjectRequest getReq = new GetObjectRequest(bucketName, objectKey);
                getReq.PartNumber = partNumber;
                config?.Invoke(getReq);

                GetObjectResponse getResp = await operations.GetObjectAsync(getReq, token).ConfigureAwait(false);

                using (getResp.Content)
                {
                    long offset = (partNumber - 1) * partSize;
                    byte[] buffer = new byte[bufferSize];

                    while (true)
                    {
                        int read = await getResp.Content.ReadUpToAsync(buffer, 0, bufferSize, token).ConfigureAwait(false);

                        if (read > 0)
                        {
                            mutex.WaitOne();

                            output.Seek(offset, SeekOrigin.Begin);
                            await output.WriteAsync(buffer, 0, read, token).ConfigureAwait(false);
                            offset += read;

                            mutex.ReleaseMutex();
                        }
                        else
                            break;
                    }
                }

                return getResp;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}