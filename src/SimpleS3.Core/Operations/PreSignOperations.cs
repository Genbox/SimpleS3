using System;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.PreSigned;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Operations
{
    public class PreSignedObjectOperations : IPreSignedObjectOperations
    {
        private readonly IPreSignRequestHandler _preSignedHandler;
        private readonly IRequestHandler _handler;

        public PreSignedObjectOperations(IPreSignRequestHandler preSignedHandler, IRequestHandler handler)
        {
            _preSignedHandler = preSignedHandler;
            _handler = handler;
        }

        public string SignGetObject(GetObjectRequest request, TimeSpan expiresIn)
        {
            return _preSignedHandler.SignRequest(request, expiresIn);
        }

        public string SignDeleteObject(DeleteObjectRequest request, TimeSpan expiresIn)
        {
            return _preSignedHandler.SignRequest(request, expiresIn);
        }

        public string SignPutObject(PutObjectRequest request, TimeSpan expiresIn)
        {
            return _preSignedHandler.SignRequest(request, expiresIn);
        }

        public string SignHeadObject(HeadObjectRequest request, TimeSpan expiresIn)
        {
            return _preSignedHandler.SignRequest(request, expiresIn);
        }

        public Task<GetObjectResponse> SendPreSignedGetObjectAsync(PreSignedGetObjectRequest request, CancellationToken token = default)
        {
            return _handler.SendRequestAsync<PreSignedGetObjectRequest, GetObjectResponse>(request, token);
        }

        public Task<DeleteObjectResponse> SendPreSignedDeleteObjectAsync(PreSignedDeleteObjectRequest request, CancellationToken token = default)
        {
            return _handler.SendRequestAsync<PreSignedDeleteObjectRequest, DeleteObjectResponse>(request, token);
        }

        public Task<PutObjectResponse> SendPreSignedPutObjectAsync(PreSignedPutObjectRequest request, CancellationToken token = default)
        {
            return _handler.SendRequestAsync<PreSignedPutObjectRequest, PutObjectResponse>(request, token);
        }

        public Task<HeadObjectResponse> SendPreSignedHeadObjectAsync(PreSignedHeadObjectRequest request, CancellationToken token = default)
        {
            return _handler.SendRequestAsync<PreSignedHeadObjectRequest, HeadObjectResponse>(request, token);
        }
    }
}