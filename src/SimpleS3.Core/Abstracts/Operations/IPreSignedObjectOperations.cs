using System;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.PreSigned;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Abstracts.Operations
{
    public interface IPreSignedObjectOperations
    {
        string SignGetObject(GetObjectRequest request, TimeSpan expiresIn);
        string SignDeleteObject(DeleteObjectRequest request, TimeSpan expiresIn);
        string SignPutObject(PutObjectRequest request, TimeSpan expiresIn);
        string SignHeadObject(HeadObjectRequest request, TimeSpan expiresIn);
        Task<GetObjectResponse> SendPreSignedGetObjectAsync(PreSignedGetObjectRequest request, CancellationToken token = default);
        Task<DeleteObjectResponse> SendPreSignedDeleteObjectAsync(PreSignedDeleteObjectRequest request, CancellationToken token = default);
        Task<PutObjectResponse> SendPreSignedPutObjectAsync(PreSignedPutObjectRequest request, CancellationToken token = default);
        Task<HeadObjectResponse> SendPreSignedHeadObjectAsync(PreSignedHeadObjectRequest request, CancellationToken token = default);
    }
}