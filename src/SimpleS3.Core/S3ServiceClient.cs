using System;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Requests.Service;
using Genbox.SimpleS3.Core.Responses.Service;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core
{
    [PublicAPI]
    public class S3ServiceClient : IS3ServiceClient
    {
        private readonly IServiceOperations _operations;

        public S3ServiceClient(IServiceOperations operations)
        {
            _operations = operations;
        }

        public Task<GetServiceResponse> GetServiceAsync(Action<GetServiceRequest> config = null, CancellationToken token = default)
        {
            return _operations.GetAsync(config, token);
        }
    }
}