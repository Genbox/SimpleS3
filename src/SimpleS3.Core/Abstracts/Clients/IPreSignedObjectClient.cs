using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.PreSigned;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Abstracts.Clients
{
    public interface IPreSignedObjectClient
    {
        IPreSignedObjectOperations PreSignedObjectOperations { get; }
        string SignPutObjectRequest(PutObjectRequest request, TimeSpan expires);
        Task<PutObjectResponse> PutObjectAsync(string url, Stream? content, Action<PreSignedPutObjectRequest>? config = null, CancellationToken token = default);
        string SignGetObjectRequest(GetObjectRequest request, TimeSpan expires);
        Task<GetObjectResponse> GetObjectAsync(string url, Action<PreSignedGetObjectRequest>? config = null, CancellationToken token = default);
        string SignDeleteObjectRequest(DeleteObjectRequest request, TimeSpan expires);
        Task<DeleteObjectResponse> DeleteObjectAsync(string url, Action<PreSignedDeleteObjectRequest>? config = null, CancellationToken token = default);
        string SignHeadObjectRequest(HeadObjectRequest request, TimeSpan expires);
        Task<HeadObjectResponse> HeadObjectAsync(string url, Action<PreSignedHeadObjectRequest>? config = null, CancellationToken token = default);
    }
}