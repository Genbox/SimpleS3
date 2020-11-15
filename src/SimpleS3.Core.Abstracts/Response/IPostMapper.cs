using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Abstracts.Response
{
    public interface IPostMapper { }

    public interface IPostMapper<in TRequest, in TResponse> : IPostMapper where TRequest : IRequest where TResponse : IResponse
    {
        void PostMap(Config config, TRequest request, TResponse response);
    }
}