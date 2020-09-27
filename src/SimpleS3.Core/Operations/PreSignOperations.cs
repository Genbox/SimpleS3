using System.Threading;
using System.Threading.Tasks;
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

        public Task<string> SignGetObjectAsync(GetObjectRequest request, CancellationToken token = default)
        {
            return _requestHandler.SignRequestAsync(request, token);
        }
    }
}
