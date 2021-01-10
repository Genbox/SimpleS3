using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.Signed;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Clients
{
    internal class SignedObjectClient : ISignedObjectClient
    {
        private readonly ISignedObjectOperations _operations;

        public SignedObjectClient(ISignedObjectOperations operations)
        {
            _operations = operations;
        }

        public string SignPutObject(string bucketName, string objectKey, Stream? content, TimeSpan expires, Action<PutObjectRequest>? config = null)
        {
            PutObjectRequest request = new PutObjectRequest(bucketName, objectKey, content);
            config?.Invoke(request);

            return _operations.SignPutObject(request, expires);
        }

        public Task<PutObjectResponse> PutObjectAsync(string url, Stream? content, Action<SignedPutObjectRequest>? config = null, CancellationToken token = default)
        {
            SignedPutObjectRequest request = new SignedPutObjectRequest(url, content);
            config?.Invoke(request);

            return _operations.SendPreSignedPutObjectAsync(request, token);
        }

        public string SignGetObject(string bucketName, string objectKey, TimeSpan expires, Action<GetObjectRequest>? config = null)
        {
            GetObjectRequest request = new GetObjectRequest(bucketName, objectKey);
            config?.Invoke(request);

            return _operations.SignGetObject(request, expires);
        }

        public Task<GetObjectResponse> GetObjectAsync(string url, Action<SignedGetObjectRequest>? config = null, CancellationToken token = default)
        {
            SignedGetObjectRequest request = new SignedGetObjectRequest(url);
            config?.Invoke(request);

            return _operations.SendPreSignedGetObjectAsync(request, token);
        }

        public string SignDeleteObject(string bucketName, string objectKey, TimeSpan expires, Action<DeleteObjectRequest>? config = null)
        {
            DeleteObjectRequest request = new DeleteObjectRequest(bucketName, objectKey);
            config?.Invoke(request);

            return _operations.SignDeleteObject(request, expires);
        }

        public Task<DeleteObjectResponse> DeleteObjectAsync(string url, Action<SignedDeleteObjectRequest>? config = null, CancellationToken token = default)
        {
            SignedDeleteObjectRequest request = new SignedDeleteObjectRequest(url);
            config?.Invoke(request);

            return _operations.SendPreSignedDeleteObjectAsync(request, token);
        }

        public string SignHeadObject(string bucketName, string objectKey, TimeSpan expires, Action<HeadObjectRequest>? config = null)
        {
            HeadObjectRequest request = new HeadObjectRequest(bucketName, objectKey);
            config?.Invoke(request);

            return _operations.SignHeadObject(request, expires);
        }

        public Task<HeadObjectResponse> HeadObjectAsync(string url, Action<SignedHeadObjectRequest>? config = null, CancellationToken token = default)
        {
            SignedHeadObjectRequest request = new SignedHeadObjectRequest(url);
            config?.Invoke(request);

            return _operations.SendPreSignedHeadObjectAsync(request, token);
        }
    }
}