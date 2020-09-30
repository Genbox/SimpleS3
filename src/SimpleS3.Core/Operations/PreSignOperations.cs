using System;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Operations
{
    public class PreSignedObjectOperations : IPreSignedObjectOperations
    {
        private readonly IPreSignRequestHandler _requestHandler;

        public PreSignedObjectOperations(IPreSignRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        public string SignGetObject(GetObjectRequest request, TimeSpan expiresIn)
        {
            return _requestHandler.SignRequest(request, expiresIn);
        }

        public string SignDeleteObject(DeleteObjectRequest request, TimeSpan expiresIn)
        {
            return _requestHandler.SignRequest(request, expiresIn);
        }

        public string SignPutObject(PutObjectRequest request, TimeSpan expiresIn)
        {
            return _requestHandler.SignRequest(request, expiresIn);
        }

        public string SignHeadObject(HeadObjectRequest request, TimeSpan expiresIn)
        {
            return _requestHandler.SignRequest(request, expiresIn);
        }
    }
}