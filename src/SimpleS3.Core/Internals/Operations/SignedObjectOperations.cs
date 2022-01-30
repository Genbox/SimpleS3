using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.Signed;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Operations;

internal class SignedObjectOperations : ISignedObjectOperations
{
    private readonly IRequestHandler _handler;
    private readonly ISignedRequestHandler _signedHandler;

    public SignedObjectOperations(ISignedRequestHandler signedHandler, IRequestHandler handler)
    {
        _signedHandler = signedHandler;
        _handler = handler;
    }

    public string SignGetObject(GetObjectRequest request, TimeSpan expiresIn) => _signedHandler.SignRequest(request, expiresIn);

    public string SignDeleteObject(DeleteObjectRequest request, TimeSpan expiresIn) => _signedHandler.SignRequest(request, expiresIn);

    public string SignPutObject(PutObjectRequest request, TimeSpan expiresIn) => _signedHandler.SignRequest(request, expiresIn);

    public string SignHeadObject(HeadObjectRequest request, TimeSpan expiresIn) => _signedHandler.SignRequest(request, expiresIn);

    public Task<GetObjectResponse> SendPreSignedGetObjectAsync(SignedGetObjectRequest request, CancellationToken token = default) => _handler.SendRequestAsync<SignedGetObjectRequest, GetObjectResponse>(request, token);

    public Task<DeleteObjectResponse> SendPreSignedDeleteObjectAsync(SignedDeleteObjectRequest request, CancellationToken token = default) => _handler.SendRequestAsync<SignedDeleteObjectRequest, DeleteObjectResponse>(request, token);

    public Task<PutObjectResponse> SendPreSignedPutObjectAsync(SignedPutObjectRequest request, CancellationToken token = default) => _handler.SendRequestAsync<SignedPutObjectRequest, PutObjectResponse>(request, token);

    public Task<HeadObjectResponse> SendPreSignedHeadObjectAsync(SignedHeadObjectRequest request, CancellationToken token = default) => _handler.SendRequestAsync<SignedHeadObjectRequest, HeadObjectResponse>(request, token);
}