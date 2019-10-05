using System;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Requests.Service;
using Genbox.SimpleS3.Core.Responses.Service;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Operations
{
    [PublicAPI]
    public class ServiceOperations : IServiceOperations
    {
        private readonly IRequestHandler _requestHandler;

        public ServiceOperations(IRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        public Task<GetServiceResponse> GetAsync(Action<GetServiceRequest> config = null, CancellationToken token = default)
        {
            GetServiceRequest req = new GetServiceRequest();
            config?.Invoke(req);

            return _requestHandler.SendRequestAsync<GetServiceRequest, GetServiceResponse>(req, token);
        }
    }
}