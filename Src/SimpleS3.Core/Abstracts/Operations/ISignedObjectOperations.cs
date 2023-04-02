using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.Signed;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Abstracts.Operations;

public interface ISignedObjectOperations
{
    string SignGetObject(GetObjectRequest request, TimeSpan expiresIn);
    string SignDeleteObject(DeleteObjectRequest request, TimeSpan expiresIn);
    string SignPutObject(PutObjectRequest request, TimeSpan expiresIn);
    string SignHeadObject(HeadObjectRequest request, TimeSpan expiresIn);
    Task<GetObjectResponse> SendPreSignedGetObjectAsync(SignedGetObjectRequest request, CancellationToken token = default);
    Task<DeleteObjectResponse> SendPreSignedDeleteObjectAsync(SignedDeleteObjectRequest request, CancellationToken token = default);
    Task<PutObjectResponse> SendPreSignedPutObjectAsync(SignedPutObjectRequest request, CancellationToken token = default);
    Task<HeadObjectResponse> SendPreSignedHeadObjectAsync(SignedHeadObjectRequest request, CancellationToken token = default);
}