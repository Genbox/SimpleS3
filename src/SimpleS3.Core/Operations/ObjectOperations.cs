using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Requests.Objects;
using Genbox.SimpleS3.Core.Responses.Objects;
using Genbox.SimpleS3.Utils;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Operations
{
    [PublicAPI]
    public class ObjectOperations : IObjectOperations
    {
        private readonly IRequestHandler _requestHandler;

        public ObjectOperations(IRequestHandler requestHandler, IEnumerable<IRequestWrapper> requestWrappers, IEnumerable<IResponseWrapper> responseWrappers)
        {
            _requestHandler = requestHandler;

            if (requestWrappers != null)
                RequestWrappers = requestWrappers.ToList();

            if (responseWrappers != null)
                ResponseWrappers = responseWrappers.ToList();
        }

        public IList<IRequestWrapper> RequestWrappers { get; }
        public IList<IResponseWrapper> ResponseWrappers { get; }

        public Task<DeleteObjectResponse> DeleteObjectAsync(DeleteObjectRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<DeleteObjectRequest, DeleteObjectResponse>(request, token);
        }

        public Task<HeadObjectResponse> HeadObjectAsync(HeadObjectRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<HeadObjectRequest, HeadObjectResponse>(request, token);
        }

        public Task<CreateMultipartUploadResponse> CreateMultipartUploadAsync(CreateMultipartUploadRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<CreateMultipartUploadRequest, CreateMultipartUploadResponse>(request, token);
        }

        public Task<UploadPartResponse> UploadPartAsync(UploadPartRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<UploadPartRequest, UploadPartResponse>(request, token);
        }

        public Task<ListPartsResponse> ListPartsAsync(ListPartsRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<ListPartsRequest, ListPartsResponse>(request, token);
        }

        public Task<CompleteMultipartUploadResponse> CompleteMultipartUploadAsync(CompleteMultipartUploadRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<CompleteMultipartUploadRequest, CompleteMultipartUploadResponse>(request, token);
        }

        public Task<AbortMultipartUploadResponse> AbortMultipartUploadAsync(AbortMultipartUploadRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<AbortMultipartUploadRequest, AbortMultipartUploadResponse>(request, token);
        }

        public Task<DeleteObjectsResponse> DeleteObjectsAsync(DeleteObjectsRequest request, CancellationToken token = default)
        {
            return _requestHandler.SendRequestAsync<DeleteObjectsRequest, DeleteObjectsResponse>(request, token);
        }

        public Task<PutObjectResponse> PutObjectAsync(PutObjectRequest request, CancellationToken token = default)
        {
            Validator.RequireNotNull(request, nameof(request));

            Stream data = request.Content;

            if (RequestWrappers != null)
            {
                foreach (IRequestWrapper wrapper in RequestWrappers)
                {
                    if (wrapper.IsSupported(request))
                        data = wrapper.Wrap(data, request);
                }
            }

            //Make sure we overwrite the stream reference with the wrapped one
            request.Content = data;

            return _requestHandler.SendRequestAsync<PutObjectRequest, PutObjectResponse>(request, token);
        }

        public async Task<GetObjectResponse> GetObjectAsync(GetObjectRequest request, CancellationToken token = default)
        {
            GetObjectResponse response = await _requestHandler.SendRequestAsync<GetObjectRequest, GetObjectResponse>(request, token).ConfigureAwait(false);

            foreach (IResponseWrapper wrapper in ResponseWrappers)
                response.Content.InputStream = wrapper.Wrap(response.Content.AsStream(), response);

            return response;
        }

        public async IAsyncEnumerable<UploadPartResponse> MultipartUploadAsync(CreateMultipartUploadRequest req, Stream data, int partSize = 16777216, int numParallelParts = 4, [EnumeratorCancellation] CancellationToken token = default)
        {
            Validator.RequireNotNull(req, nameof(req));
            Validator.RequireNotNull(data, nameof(data));

            if (RequestWrappers != null)
            {
                foreach (IRequestWrapper wrapper in RequestWrappers)
                {
                    if (wrapper.IsSupported(req))
                        data = wrapper.Wrap(data, req);
                }
            }

            string bucket = req.BucketName;
            string resource = req.Resource;

            CreateMultipartUploadResponse initResp = await CreateMultipartUploadAsync(req, token).ConfigureAwait(false);

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

                    uploads.Enqueue(UploadPartAsync(bucket, resource, partData, i, initResp.UploadId, semaphore, token));

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

                CompleteMultipartUploadRequest completeReq = new CompleteMultipartUploadRequest(bucket, resource, initResp.UploadId, responses);
                CompleteMultipartUploadResponse completeResp = await CompleteMultipartUploadAsync(completeReq, token).ConfigureAwait(false);

                if (!completeResp.IsSuccess)
                    throw new RequestException(completeResp.StatusCode, "CompleteMultipartUploadRequest was unsuccessful");
            }
        }

        public async IAsyncEnumerable<GetObjectResponse> MultipartDownloadAsync(string bucketName, string resource, Stream output, int bufferSize = 16777216, int numParallelParts = 4, Action<GetObjectRequest> config = null, [EnumeratorCancellation] CancellationToken token = default)
        {
            Validator.RequireNotNull(output, nameof(output));

            //Use a HEAD request on the resource to determine if the file was originally uploaded with multipart
            HeadObjectRequest headReq = new HeadObjectRequest(bucketName, resource);
            headReq.PartNumber = 1;

            HeadObjectResponse headResp = await HeadObjectAsync(headReq, token).ConfigureAwait(false);

            Queue<Task<GetObjectResponse>> queue = new Queue<Task<GetObjectResponse>>();

            if (headResp.NumberOfParts == null)
            {
                GetObjectRequest getReq = new GetObjectRequest(bucketName, resource);
                config?.Invoke(getReq);

                GetObjectResponse getResp = await GetObjectAsync(getReq, token).ConfigureAwait(false);

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

                        queue.Enqueue(DownloadPartAsync(bucketName, resource, output, headResp.ContentLength, i, bufferSize, semaphore, mutex, token, config));
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

        private async Task<GetObjectResponse> DownloadPartAsync(string bucketName, string resource, Stream output, long partSize, int partNumber, int bufferSize, SemaphoreSlim semaphore, Mutex mutex, CancellationToken token, Action<GetObjectRequest> config)
        {
            try
            {
                GetObjectRequest getReq = new GetObjectRequest(bucketName, resource);
                getReq.PartNumber = partNumber;
                config?.Invoke(getReq);

                GetObjectResponse getResp = await GetObjectAsync(getReq, token).ConfigureAwait(false);

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

        private async Task<UploadPartResponse> UploadPartAsync(string bucketName, string resource, byte[] data, int partNumber, string uploadId, SemaphoreSlim semaphore, CancellationToken token)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    UploadPartRequest req = new UploadPartRequest(bucketName, resource, partNumber, uploadId, ms);
                    return await UploadPartAsync(req, token).ConfigureAwait(false);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}