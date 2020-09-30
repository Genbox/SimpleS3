using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.PreSigned;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core
{
    [PublicAPI]
    public class S3PreSignedObjectClient : IPreSignedObjectClient
    {
        public S3PreSignedObjectClient(IPreSignedObjectOperations operations)
        {
            PreSignedObjectOperations = operations;
        }

        public IPreSignedObjectOperations PreSignedObjectOperations { get; }

        public string SignPutObjectRequest(PutObjectRequest request, TimeSpan expires)
        {
            return PreSignedObjectOperations.SignPutObject(request, expires);
        }

        public Task<PutObjectResponse> PutObjectAsync(string url, Stream? content, Action<PreSignedPutObjectRequest>? config = null, CancellationToken token = default)
        {
            PreSignedPutObjectRequest request = new PreSignedPutObjectRequest(url, content);
            config?.Invoke(request);

            return PreSignedObjectOperations.SendPreSignedPutObjectAsync(request, token);
        }

        public string SignGetObjectRequest(GetObjectRequest request, TimeSpan expires)
        {
            return PreSignedObjectOperations.SignGetObject(request, expires);
        }

        public Task<GetObjectResponse> GetObjectAsync(string url, Action<PreSignedGetObjectRequest>? config = null, CancellationToken token = default)
        {
            PreSignedGetObjectRequest request = new PreSignedGetObjectRequest(url);
            config?.Invoke(request);

            return PreSignedObjectOperations.SendPreSignedGetObjectAsync(request, token);
        }

        public string SignDeleteObjectRequest(DeleteObjectRequest request, TimeSpan expires)
        {
            return PreSignedObjectOperations.SignDeleteObject(request, expires);
        }

        public Task<DeleteObjectResponse> DeleteObjectAsync(string url, Action<PreSignedDeleteObjectRequest>? config = null, CancellationToken token = default)
        {
            PreSignedDeleteObjectRequest request = new PreSignedDeleteObjectRequest(url);
            config?.Invoke(request);

            return PreSignedObjectOperations.SendPreSignedDeleteObjectAsync(request, token);
        }

        public string SignHeadObjectRequest(HeadObjectRequest request, TimeSpan expires)
        {
            return PreSignedObjectOperations.SignHeadObject(request, expires);
        }

        public Task<HeadObjectResponse> HeadObjectAsync(string url, Action<PreSignedHeadObjectRequest>? config = null, CancellationToken token = default)
        {
            PreSignedHeadObjectRequest request = new PreSignedHeadObjectRequest(url);
            config?.Invoke(request);

            return PreSignedObjectOperations.SendPreSignedHeadObjectAsync(request, token);
        }
    }
}