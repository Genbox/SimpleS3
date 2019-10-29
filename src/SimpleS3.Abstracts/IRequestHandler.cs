using System.Threading;
using System.Threading.Tasks;

namespace Genbox.SimpleS3.Abstracts
{
    public interface IRequestHandler
    {
        Task<TResp> SendRequestAsync<TReq, TResp>(TReq request, CancellationToken cancellationToken = default) where TResp : IResponse, new() where TReq : IRequest;
    }
}