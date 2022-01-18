using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Factories;

public interface IPostMapperFactory
{
    void PostMap<TRequest, TResponse>(SimpleS3Config config, TRequest request, TResponse response) where TRequest : IRequest where TResponse : IResponse;
}