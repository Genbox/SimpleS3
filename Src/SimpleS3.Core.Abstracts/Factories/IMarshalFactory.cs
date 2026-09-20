using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Abstracts.Factories;

/// <summary>Provides the <c>IMarshalFactory</c> API.</summary>
public interface IMarshalFactory
{
    /// <summary>Provides the <c>MarshalRequest&lt;TRequest&gt;</c> API.</summary>
    Stream? MarshalRequest<TRequest>(SimpleS3Config config, TRequest request) where TRequest : IRequest;

    /// <summary>Provides the <c>MarshalResponse&lt;TResponse&gt;</c> API.</summary>
    void MarshalResponse<TResponse>(SimpleS3Config config, TResponse response, IDictionary<string, string> headers, ContentStream responseStream) where TResponse : IResponse;
}