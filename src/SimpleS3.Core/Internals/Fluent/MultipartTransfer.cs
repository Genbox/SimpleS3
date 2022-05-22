using System.Runtime.CompilerServices;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
using Genbox.SimpleS3.Core.Abstracts.Wrappers;
using Genbox.SimpleS3.Core.Common.Exceptions;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Fluent;

internal class MultipartTransfer : IMultipartTransfer
{
    private readonly IMultipartClient _multipartClient;
    private readonly IMultipartOperations _multipartOperations;
    private readonly IObjectClient _objectClient;
    private readonly IEnumerable<IRequestWrapper> _requestWrappers;

    public MultipartTransfer(IObjectClient objectClient, IMultipartClient multipartClient, IMultipartOperations multipartOperations, IEnumerable<IRequestWrapper> requestWrappers)
    {
        _objectClient = objectClient;
        _multipartClient = multipartClient;
        _multipartOperations = multipartOperations;
        _requestWrappers = requestWrappers;
    }

    public async IAsyncEnumerable<GetObjectResponse> MultipartDownloadAsync(string bucketName, string objectKey, Stream output, int bufferSize = 16777216, int numParallelParts = 4, Action<GetObjectRequest>? config = null, [EnumeratorCancellation]CancellationToken token = default)
    {
        Validator.RequireNotNull(output, nameof(output));

        //Use a HEAD request on the object key to determine if the file was originally uploaded with multipart
        HeadObjectResponse headResp = await _objectClient.HeadObjectAsync(bucketName, objectKey, req => req.PartNumber = 1, token).ConfigureAwait(false);

        Queue<Task<GetObjectResponse>> queue = new Queue<Task<GetObjectResponse>>();

        if (headResp.NumberOfParts == null)
        {
            GetObjectResponse getResp = await _objectClient.GetObjectAsync(bucketName, objectKey, config, token).ConfigureAwait(false);

            if (!getResp.IsSuccess)
                throw new S3RequestException(getResp);

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

                    queue.Enqueue(DownloadPartAsync(bucketName, objectKey, output, headResp.ContentLength, i, bufferSize, semaphore, mutex, config, token));
                }

                while (queue.TryDequeue(out Task<GetObjectResponse>? task))
                {
                    if (token.IsCancellationRequested)
                        yield break;

                    yield return await task!.ConfigureAwait(false);
                }
            }
        }
    }

    public Task<CompleteMultipartUploadResponse> MultipartUploadAsync(string bucketName, string objectKey, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<CreateMultipartUploadRequest>? config = null, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default)
    {
        CreateMultipartUploadRequest req = new CreateMultipartUploadRequest(bucketName, objectKey);
        config?.Invoke(req);

        return MultipartUploadAsync(req, data, partSize, numParallelParts, onPartResponse, token);
    }

    public async Task<CompleteMultipartUploadResponse> MultipartUploadAsync(CreateMultipartUploadRequest req, Stream data, int partSize = 16777216, int numParallelParts = 4, Action<UploadPartResponse>? onPartResponse = null, CancellationToken token = default)
    {
        Validator.RequireNotNull(req, nameof(req));
        Validator.RequireNotNull(data, nameof(data));

        foreach (IRequestWrapper wrapper in _requestWrappers)
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

            CreateMultipartUploadResponse initResp = await _multipartOperations.CreateMultipartUploadAsync(req, token).ConfigureAwait(false);

            if (token.IsCancellationRequested)
                return new CompleteMultipartUploadResponse { BucketName = bucket, ObjectKey = objectKey };

            if (!initResp.IsSuccess)
                throw new S3RequestException(initResp, "CreateMultipartUploadRequest was unsuccessful");

            IEnumerable<ArraySegment<byte>> chunks = ReadChunks(data, partSize);

            int partNumber = 0;

            IEnumerable<UploadPartResponse> responses = await ParallelHelper.ExecuteAsync(chunks, async (bytes, innerToken) =>
            {
                Interlocked.Increment(ref partNumber);

                using (MemoryStream ms = new MemoryStream(bytes.Array!, 0, bytes.Count))
                {
                    UploadPartResponse resp = await _multipartClient.UploadPartAsync(bucket, objectKey, partNumber, initResp.UploadId, ms, uploadPart =>
                    {
                        uploadPart.SseCustomerAlgorithm = req.SseCustomerAlgorithm;
                        uploadPart.SseCustomerKey = encryptionKey;
                        uploadPart.SseCustomerKeyMd5 = req.SseCustomerKeyMd5;
                    }, innerToken).ConfigureAwait(false);
                    onPartResponse?.Invoke(resp);
                    return resp;
                }
            }, numParallelParts, token).ConfigureAwait(false);

            CompleteMultipartUploadResponse completeResp = await _multipartClient.CompleteMultipartUploadAsync(bucket, objectKey, initResp.UploadId, responses.OrderBy(x => x.PartNumber), null, token).ConfigureAwait(false);

            return completeResp;
        }
        finally
        {
            if (encryptionKey != null)
                Array.Clear(encryptionKey, 0, encryptionKey.Length);
        }
    }

    private async Task<GetObjectResponse> DownloadPartAsync(string bucketName, string objectKey, Stream output, long partSize, int partNumber, int bufferSize, SemaphoreSlim semaphore, Mutex mutex, Action<GetObjectRequest>? config, CancellationToken token)
    {
        try
        {
            using GetObjectResponse getResp = await _objectClient.GetObjectAsync(bucketName, objectKey, req =>
            {
                req.PartNumber = partNumber;
                config?.Invoke(req);
            }, token).ConfigureAwait(false);

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

            return getResp;
        }
        finally
        {
            semaphore.Release();
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