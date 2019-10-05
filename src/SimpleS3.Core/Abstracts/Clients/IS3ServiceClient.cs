using System;
using System.Threading;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Requests.Service;
using Genbox.SimpleS3.Core.Responses.Service;

namespace Genbox.SimpleS3.Core.Abstracts.Clients
{
    public interface IS3ServiceClient
    {
        Task<GetServiceResponse> GetServiceAsync(Action<GetServiceRequest> config = null, CancellationToken token = default);
    }
}