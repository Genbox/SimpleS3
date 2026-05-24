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
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Fluent;

internal class MultipartTransfer(IObjectClient objectClient, IMultipartClient multipartClient, IMultipartOperations multipartOperations, IEnumerable<IRequestWrapper> requestWrappers) : IMultipartTransfer
{
    public async IAsyncEnumerable<GetObjectResponse> MultipartDownloadAsync(string bucketName, string objectKey, Stream output, int bufferSize = 16777216, int numParallelParts = 4, Action<GetObjectRequest>? config = null, [EnumeratorCancellation]CancellationToken token = default)
    {
        Validator.RequireNotNull(output);
        if (bufferSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(bufferSize), "Buffer size must be greater than zero.");

        if (numParallelParts <= 0)
            throw new ArgumentOutOfRangeException(nameof(numParallelParts), "Number of parallel parts must be greater than zero.");

        //Use a HEAD request on the object key to determine if the file was originally uploaded with multipart
        HeadObjectResponse headResp = await objectClient.HeadObjectAsync(bucketName, objectKey, req => req.PartNumber = 1, token).ConfigureAwait(false);

        if (headResp.NumberOfParts == null)
        {
            GetObjectResponse getResp = await objectClient.GetObjectAsync(bucketName, objectKey, config, token).ConfigureAwait(false);

            if (!getResp.IsSuccess)
                throw new S3RequestException(getResp);

            await getResp.Content.CopyToAsync(output, 81920, token).ConfigureAwait(false);

            yield return getResp;
        }
        else
        {
            int parts = headResp.NumberOfParts.Value;
            long[] partOffsets = new long[parts];
            long nextOffset = headResp.ContentLength;

            for (int i = 2; i <= parts; i++)
            {
                partOffsets[i - 1] = nextOffset;
                HeadObjectResponse partHeadResp = await objectClient.HeadObjectAsync(bucketName, objectKey, req => req.PartNumber = i, token).ConfigureAwait(false);
                nextOffset += partHeadResp.ContentLength;
            }

            using CancellationTokenSource cancellation = CancellationTokenSource.CreateLinkedTokenSource(token);
            using SemaphoreSlim semaphore = new SemaphoreSlim(numParallelParts);
            using SemaphoreSlim writeLock = new SemaphoreSlim(1, 1);
            List<Task<GetObjectResponse>> tasks = new List<Task<GetObjectResponse>>(parts);

            try
            {
                for (int i = 1; i <= parts; i++)
                {
                    await semaphore.WaitAsync(cancellation.Token).ConfigureAwait(false);
                    tasks.Add(DownloadPartAsync(bucketName, objectKey, output, partOffsets[i - 1], i, bufferSize, semaphore, writeLock, config, cancellation.Token));
                }

                foreach (Task<GetObjectResponse> task in tasks)
                    yield return await task.ConfigureAwait(false);
            }
            finally
            {
                cancellation.Cancel();

                foreach (Task<GetObjectResponse> task in tasks)
                {
                    try
                    {
                        await task.ConfigureAwait(false);
                    }
                    catch
                    {
                        // Preserve the original cancellation or download failure.
                    }
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
        Validator.RequireNotNull(req);
        Validator.RequireNotNull(data);
        if (partSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(partSize), "Part size must be greater than zero.");

        if (numParallelParts <= 0)
            throw new ArgumentOutOfRangeException(nameof(numParallelParts), "Number of parallel parts must be greater than zero.");

        if (data.CanSeek && data.Length - data.Position > partSize && partSize < 5 * 1024 * 1024)
            throw new ArgumentOutOfRangeException(nameof(partSize), "S3 multipart upload parts must be at least 5 MiB except for the final part.");

#pragma warning disable IDISP003 // Wrappers transfer stream ownership to the returned wrapper.
        foreach (IRequestWrapper wrapper in requestWrappers)
        {
            if (wrapper.IsSupported(req))
                data = wrapper.Wrap(data, req);
        }
#pragma warning restore IDISP003

        string bucket = req.BucketName;
        string objectKey = req.ObjectKey;

        byte[]? encryptionKey = null;
        string? uploadId = null;
        bool completed = false;

        try
        {
            if (req.SseCustomerKey != null)
            {
                encryptionKey = new byte[req.SseCustomerKey.Length];
                Array.Copy(req.SseCustomerKey, 0, encryptionKey, 0, encryptionKey.Length);
            }

            CreateMultipartUploadResponse initResp = await multipartOperations.CreateMultipartUploadAsync(req, token).ConfigureAwait(false);

            if (!initResp.IsSuccess)
                throw new S3RequestException(initResp, "CreateMultipartUploadRequest was unsuccessful");

            uploadId = initResp.UploadId;
            token.ThrowIfCancellationRequested();

            IEnumerable<ArraySegment<byte>> chunks = ReadChunks(data, partSize);

            int partNumber = 0;

            IEnumerable<UploadPartResponse> responses = await ParallelHelper.ExecuteAsync(chunks, async (bytes, innerToken) =>
            {
                int currentPartNumber = Interlocked.Increment(ref partNumber);

                using MemoryStream ms = new MemoryStream(bytes.Array!, 0, bytes.Count);
                UploadPartResponse resp = await multipartClient.UploadPartAsync(bucket, objectKey, currentPartNumber, initResp.UploadId, ms, uploadPart =>
                {
                    uploadPart.SseCustomerAlgorithm = req.SseCustomerAlgorithm;
                    uploadPart.SseCustomerKey = encryptionKey;
                    uploadPart.SseCustomerKeyMd5 = req.SseCustomerKeyMd5;
                }, innerToken).ConfigureAwait(false);

                if (!resp.IsSuccess)
                    throw new S3RequestException(resp, "UploadPartRequest was unsuccessful");

                if (string.IsNullOrEmpty(resp.ETag))
                    throw new S3RequestException(resp, "UploadPartResponse did not include an ETag");

                onPartResponse?.Invoke(resp);
                return resp;
            }, numParallelParts, token).ConfigureAwait(false);

            CompleteMultipartUploadResponse completeResp = await multipartClient.CompleteMultipartUploadAsync(bucket, objectKey, initResp.UploadId, responses.Select(x => new S3PartInfo(x.ETag, x.PartNumber)).OrderBy(x => x.PartNumber), null, token).ConfigureAwait(false);
            completed = completeResp.IsSuccess;

            return completeResp;
        }
        finally
        {
            if (!completed && uploadId != null)
            {
                try
                {
                    await multipartClient.AbortMultipartUploadAsync(bucket, objectKey, uploadId, null, CancellationToken.None).ConfigureAwait(false);
                }
                catch
                {
                    // Preserve the original upload failure.
                }
            }

            if (encryptionKey != null)
                Array.Clear(encryptionKey, 0, encryptionKey.Length);
        }
    }

    private async Task<GetObjectResponse> DownloadPartAsync(string bucketName, string objectKey, Stream output, long offset, int partNumber, int bufferSize, SemaphoreSlim semaphore, SemaphoreSlim writeLock, Action<GetObjectRequest>? config, CancellationToken token)
    {
        try
        {
            GetObjectResponse getResp = await objectClient.GetObjectAsync(bucketName, objectKey, req =>
            {
                req.PartNumber = partNumber;
                config?.Invoke(req);
            }, token).ConfigureAwait(false);

            using (getResp.Content)
            {
                byte[] buffer = new byte[bufferSize];

                while (true)
                {
                    int read = await getResp.Content.ReadUpToAsync(buffer, 0, bufferSize, token).ConfigureAwait(false);

                    if (read > 0)
                    {
                        await writeLock.WaitAsync(token).ConfigureAwait(false);

                        try
                        {
                            output.Seek(offset, SeekOrigin.Begin);
                            await output.WriteAsync(buffer, 0, read, token).ConfigureAwait(false);
                            offset += read;
                        }
                        finally
                        {
                            writeLock.Release();
                        }
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