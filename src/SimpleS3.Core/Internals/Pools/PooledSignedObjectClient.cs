#if COMMERCIAL
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.Signed;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Pools
{
    internal class PooledSignedObjectClient : ISignedObjectClient
    {
        public PooledSignedObjectClient(ISignedObjectOperations operations)
        {
            SignedObjectOperations = operations;
        }

        public ISignedObjectOperations SignedObjectOperations { get; }

        public string SignPutObject(string bucketName, string objectKey, Stream? content, TimeSpan expires, Action<PutObjectRequest>? config = null)
        {
            void Setup(PutObjectRequest req)
            {
                req.Initialize(bucketName, objectKey, content);
                config?.Invoke(req);
            }

            string Action(PutObjectRequest request) => SignedObjectOperations.SignPutObject(request, expires);

            return ObjectPool<PutObjectRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<PutObjectResponse> PutObjectAsync(string url, Stream? content, Action<SignedPutObjectRequest>? config = null, CancellationToken token = default)
        {
            void Setup(SignedPutObjectRequest req)
            {
                req.Initialize(url, content);
                config?.Invoke(req);
            }

            Task<PutObjectResponse> Action(SignedPutObjectRequest request) => SignedObjectOperations.SendPreSignedPutObjectAsync(request, token);

            return ObjectPool<SignedPutObjectRequest>.Shared.RentAndUse(Setup, Action);
        }

        public string SignGetObject(string bucketName, string objectKey, TimeSpan expires, Action<GetObjectRequest>? config = null)
        {
            void Setup(GetObjectRequest req)
            {
                req.Initialize(bucketName, objectKey);
                config?.Invoke(req);
            }

            string Action(GetObjectRequest request) => SignedObjectOperations.SignGetObject(request, expires);

            return ObjectPool<GetObjectRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<GetObjectResponse> GetObjectAsync(string url, Action<SignedGetObjectRequest>? config = null, CancellationToken token = default)
        {
            void Setup(SignedGetObjectRequest req)
            {
                req.Initialize(url);
                config?.Invoke(req);
            }

            Task<GetObjectResponse> Action(SignedGetObjectRequest request) => SignedObjectOperations.SendPreSignedGetObjectAsync(request, token);

            return ObjectPool<SignedGetObjectRequest>.Shared.RentAndUse(Setup, Action);
        }

        public string SignDeleteObject(string bucketName, string objectKey, TimeSpan expires, Action<DeleteObjectRequest>? config = null)
        {
            void Setup(DeleteObjectRequest req)
            {
                req.Initialize(bucketName, objectKey);
                config?.Invoke(req);
            }

            string Action(DeleteObjectRequest request) => SignedObjectOperations.SignDeleteObject(request, expires);

            return ObjectPool<DeleteObjectRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<DeleteObjectResponse> DeleteObjectAsync(string url, Action<SignedDeleteObjectRequest>? config = null, CancellationToken token = default)
        {
            void Setup(SignedDeleteObjectRequest req)
            {
                req.Initialize(url);
                config?.Invoke(req);
            }

            Task<DeleteObjectResponse> Action(SignedDeleteObjectRequest request) => SignedObjectOperations.SendPreSignedDeleteObjectAsync(request, token);

            return ObjectPool<SignedDeleteObjectRequest>.Shared.RentAndUse(Setup, Action);
        }

        public string SignHeadObject(string bucketName, string objectKey, TimeSpan expires, Action<HeadObjectRequest>? config = null)
        {
            void Setup(HeadObjectRequest req)
            {
                req.Initialize(bucketName, objectKey);
                config?.Invoke(req);
            }

            string Action(HeadObjectRequest request) => SignedObjectOperations.SignHeadObject(request, expires);

            return ObjectPool<HeadObjectRequest>.Shared.RentAndUse(Setup, Action);
        }

        public Task<HeadObjectResponse> HeadObjectAsync(string url, Action<SignedHeadObjectRequest>? config = null, CancellationToken token = default)
        {
            void Setup(SignedHeadObjectRequest req)
            {
                req.Initialize(url);
                config?.Invoke(req);
            }

            Task<HeadObjectResponse> Action(SignedHeadObjectRequest request) => SignedObjectOperations.SendPreSignedHeadObjectAsync(request, token);

            return ObjectPool<SignedHeadObjectRequest>.Shared.RentAndUse(Setup, Action);
        }
    }
}
#endif