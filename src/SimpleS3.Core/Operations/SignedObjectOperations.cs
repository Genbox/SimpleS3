using System;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.Signed;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Operations
{
    public class SignedObjectOperations : ISignedObjectOperations
    {
        private readonly ISignedRequestHandler _signedHandler;
        private readonly IRequestHandler _handler;

        public SignedObjectOperations(ISignedRequestHandler signedHandler, IRequestHandler handler)
        {
            _signedHandler = signedHandler;
            _handler = handler;
        }

        public string SignGetObject(GetObjectRequest request, TimeSpan expiresIn)
        {
            return _signedHandler.SignRequest(request, expiresIn);
        }

        public string SignDeleteObject(DeleteObjectRequest request, TimeSpan expiresIn)
        {
            return _signedHandler.SignRequest(request, expiresIn);
        }

        public string SignPutObject(PutObjectRequest request, TimeSpan expiresIn)
        {
            return _signedHandler.SignRequest(request, expiresIn);
        }

        public string SignHeadObject(HeadObjectRequest request, TimeSpan expiresIn)
        {
            return _signedHandler.SignRequest(request, expiresIn);
        }

        public Task<GetObjectResponse> SendPreSignedGetObjectAsync(SignedGetObjectRequest request, CancellationToken token = default)
        {
            return _handler.SendRequestAsync<SignedGetObjectRequest, GetObjectResponse>(request, token);
        }

        public Task<DeleteObjectResponse> SendPreSignedDeleteObjectAsync(SignedDeleteObjectRequest request, CancellationToken token = default)
        {
            return _handler.SendRequestAsync<SignedDeleteObjectRequest, DeleteObjectResponse>(request, token);
        }

        public Task<PutObjectResponse> SendPreSignedPutObjectAsync(SignedPutObjectRequest request, CancellationToken token = default)
        {
            return _handler.SendRequestAsync<SignedPutObjectRequest, PutObjectResponse>(request, token);
        }

        public Task<HeadObjectResponse> SendPreSignedHeadObjectAsync(SignedHeadObjectRequest request, CancellationToken token = default)
        {
            return _handler.SendRequestAsync<SignedHeadObjectRequest, HeadObjectResponse>(request, token);
        }
    }
}