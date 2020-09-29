using System;
using System.Threading;
using System.Threading.Tasks;
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

        public Task<string> SignGetObjectAsync(GetObjectRequest request, TimeSpan expiresIn, CancellationToken token = default)
        {
            return _requestHandler.SignRequestAsync(request, expiresIn, token);
        }

        public Task<string> SignDeleteObjectAsync(DeleteObjectRequest request, TimeSpan expiresIn, CancellationToken token = default)
        {
            return _requestHandler.SignRequestAsync(request, expiresIn, token);
        }

        public Task<string> SignPutObjectAsync(PutObjectRequest request, TimeSpan expiresIn, CancellationToken token = default)
        {
            return _requestHandler.SignRequestAsync(request, expiresIn, token);
        }

        public Task<string> SignHeadObjectAsync(HeadObjectRequest request, TimeSpan expiresIn, CancellationToken token = default)
        {
            return _requestHandler.SignRequestAsync(request, expiresIn, token);
        }
    }
}
