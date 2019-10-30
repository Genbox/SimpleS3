using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Requests.Objects;
using Genbox.SimpleS3.Core.Responses.Objects;
using Genbox.SimpleS3.Utils;

namespace Genbox.SimpleS3.Core.Internal.Helpers
{
    internal static class MultipartHelper
    {
        public static async IAsyncEnumerable<UploadPartResponse> MultipartUploadAsync(IObjectOperations operations, CreateMultipartUploadRequest req, Stream data, int partSize = 16777216, int numParallelParts = 4, [EnumeratorCancellation] CancellationToken token = default)
        {
            Validator.RequireNotNull(req, nameof(req));
            Validator.RequireNotNull(data, nameof(data));

            if (operations.RequestWrappers != null)
            {
                foreach (IRequestWrapper wrapper in operations.RequestWrappers)
                {
                    if (wrapper.IsSupported(req))
                        data = wrapper.Wrap(data, req);
                }
            }

            string bucket = req.BucketName;
            string objectKey = req.ObjectKey;

            CreateMultipartUploadResponse initResp = await operations.CreateMultipartUploadAsync(req, token).ConfigureAwait(false);

            if (token.IsCancellationRequested)
                yield break;

            if (!initResp.IsSuccess)
                throw new RequestException(initResp.StatusCode, "CreateMultipartUploadRequest was unsuccessful");

            Queue<Task<UploadPartResponse>> uploads = new Queue<Task<UploadPartResponse>>();

            using (SemaphoreSlim semaphore = new SemaphoreSlim(numParallelParts))
            {
                long offset = 0;

                for (int i = 1; offset < data.Length; i++)
                {
                    await semaphore.WaitAsync(token).ConfigureAwait(false);

                    if (token.IsCancellationRequested)
                        break;

                    long remaining = data.Length - offset;
                    long bufferSize = Math.Min(remaining, partSize);

                    byte[] partData = new byte[bufferSize];
                    await data.ReadAsync(partData, 0, partData.Length, token).ConfigureAwait(false);

                    uploads.Enqueue(UploadPartAsync(operations, bucket, objectKey, partData, i, initResp.UploadId, semaphore, token));

                    offset += partSize;
                }

                Queue<UploadPartResponse> responses = new Queue<UploadPartResponse>(uploads.Count);

                while (uploads.TryDequeue(out Task<UploadPartResponse> task))
                {
                    if (token.IsCancellationRequested)
                        yield break;

                    UploadPartResponse response = await task.ConfigureAwait(false);
                    responses.Enqueue(response);

                    yield return response;
                }

                CompleteMultipartUploadRequest completeReq = new CompleteMultipartUploadRequest(bucket, objectKey, initResp.UploadId, responses);
                CompleteMultipartUploadResponse completeResp = await operations.CompleteMultipartUploadAsync(completeReq, token).ConfigureAwait(false);

                if (!completeResp.IsSuccess)
                    throw new RequestException(completeResp.StatusCode, "CompleteMultipartUploadRequest was unsuccessful");
            }
        }

        public static async IAsyncEnumerable<GetObjectResponse> MultipartDownloadAsync(IObjectOperations operations, string bucketName, string objectKey, Stream output, int bufferSize = 16777216, int numParallelParts = 4, Action<GetObjectRequest> config = null, [EnumeratorCancellation] CancellationToken token = default)
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
                    throw new Exception();

                await getResp.Content.CopyToAsync(output, token: token).ConfigureAwait(false);

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

                        queue.Enqueue(DownloadPartAsync(operations, bucketName, objectKey, output, headResp.ContentLength, i, bufferSize, semaphore, mutex, token, config));
                    }

                    while (queue.TryDequeue(out Task<GetObjectResponse> task))
                    {
                        if (token.IsCancellationRequested)
                            yield break;

                        GetObjectResponse response = await task.ConfigureAwait(false);
                        yield return response;
                    }
                }
            }
        }

        private static async Task<GetObjectResponse> DownloadPartAsync(IObjectOperations operations, string bucketName, string objectKey, Stream output, long partSize, int partNumber, int bufferSize, SemaphoreSlim semaphore, Mutex mutex, CancellationToken token, Action<GetObjectRequest> config)
        {
            try
            {
                GetObjectRequest getReq = new GetObjectRequest(bucketName, objectKey);
                getReq.PartNumber = partNumber;
                config?.Invoke(getReq);

                GetObjectResponse getResp = await operations.GetObjectAsync(getReq, token).ConfigureAwait(false);

                using (Stream stream = getResp.Content.AsStream())
                {
                    long offset = (partNumber - 1) * partSize;
                    byte[] buffer = new byte[bufferSize];

                    while (true)
                    {
                        int read = stream.ReadUpTo(buffer, 0, bufferSize);

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

        private static async Task<UploadPartResponse> UploadPartAsync(IObjectOperations operations, string bucketName, string objectKey, byte[] data, int partNumber, string uploadId, SemaphoreSlim semaphore, CancellationToken token)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    UploadPartRequest req = new UploadPartRequest(bucketName, objectKey, partNumber, uploadId, ms);
                    return await operations.UploadPartAsync(req, token).ConfigureAwait(false);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
