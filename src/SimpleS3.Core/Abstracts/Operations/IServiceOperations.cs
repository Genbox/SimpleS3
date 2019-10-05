using System;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Requests.Service;
using Genbox.SimpleS3.Core.Responses.Service;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Operations
{
    [PublicAPI]
    public interface IServiceOperations
    {
        Task<GetServiceResponse> GetAsync(Action<GetServiceRequest> config = null, CancellationToken token = default);
    }
}