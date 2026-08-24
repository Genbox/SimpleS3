using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Factories;

/// <summary>Provides the <c>IPostMapperFactory</c> API.</summary>
public interface IPostMapperFactory
{
    /// <summary>Provides the <c>PostMap&lt;TRequest,TResponse&gt;</c> API.</summary>
    void PostMap<TRequest, TResponse>(SimpleS3Config config, TRequest request, TResponse response) where TRequest : IRequest where TResponse : IResponse;
}