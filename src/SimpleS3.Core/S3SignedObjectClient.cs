using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.Signed;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core
{
    [PublicAPI]
    public class S3SignedObjectClient : ISignedObjectClient
    {
        public S3SignedObjectClient(ISignedObjectOperations operations)
        {
            SignedObjectOperations = operations;
        }

        public ISignedObjectOperations SignedObjectOperations { get; }

        public string SignPutObject(string bucketName, string objectKey, Stream? data, TimeSpan expire, Action<PutObjectRequest>? config = null)
        {
            PutObjectRequest request = new PutObjectRequest(bucketName, objectKey, data);
            config?.Invoke(request);

            return SignedObjectOperations.SignPutObject(request, expire);
        }

        public Task<PutObjectResponse> PutObjectAsync(string url, Stream? content, Action<SignedPutObjectRequest>? config = null, CancellationToken token = default)
        {
            SignedPutObjectRequest request = new SignedPutObjectRequest(url, content);
            config?.Invoke(request);

            return SignedObjectOperations.SendPreSignedPutObjectAsync(request, token);
        }

        public string SignGetObject(string bucketName, string objectKey, TimeSpan expires, Action<GetObjectRequest>? config = null)
        {
            GetObjectRequest request = new GetObjectRequest(bucketName, objectKey);
            config?.Invoke(request);

            return SignedObjectOperations.SignGetObject(request, expires);
        }

        public Task<GetObjectResponse> GetObjectAsync(string url, Action<SignedGetObjectRequest>? config = null, CancellationToken token = default)
        {
            SignedGetObjectRequest request = new SignedGetObjectRequest(url);
            config?.Invoke(request);

            return SignedObjectOperations.SendPreSignedGetObjectAsync(request, token);
        }

        public string SignDeleteObject(string bucketName, string objectKey, TimeSpan expires, Action<DeleteObjectRequest>? config = null)
        {
            DeleteObjectRequest request = new DeleteObjectRequest(bucketName, objectKey);
            config?.Invoke(request);

            return SignedObjectOperations.SignDeleteObject(request, expires);
        }

        public Task<DeleteObjectResponse> DeleteObjectAsync(string url, Action<SignedDeleteObjectRequest>? config = null, CancellationToken token = default)
        {
            SignedDeleteObjectRequest request = new SignedDeleteObjectRequest(url);
            config?.Invoke(request);

            return SignedObjectOperations.SendPreSignedDeleteObjectAsync(request, token);
        }

        public string SignHeadObject(string bucketName, string objectKey, TimeSpan expires, Action<HeadObjectRequest>? config = null)
        {
            HeadObjectRequest request = new HeadObjectRequest(bucketName, objectKey);
            config?.Invoke(request);

            return SignedObjectOperations.SignHeadObject(request, expires);
        }

        public Task<HeadObjectResponse> HeadObjectAsync(string url, Action<SignedHeadObjectRequest>? config = null, CancellationToken token = default)
        {
            SignedHeadObjectRequest request = new SignedHeadObjectRequest(url);
            config?.Invoke(request);

            return SignedObjectOperations.SendPreSignedHeadObjectAsync(request, token);
        }
    }
}