using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Response;

/// <summary>Provides the <c>IResponseMarshal</c> API.</summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IResponseMarshal;

/// <summary>Provides the <c>IResponseMarshal&lt;in TResponse&gt;</c> API.</summary>
public interface IResponseMarshal<in TResponse> : IResponseMarshal where TResponse : IResponse
{
    /// <summary>Provides the <c>MarshalResponse</c> API.</summary>
    void MarshalResponse(SimpleS3Config config, TResponse response, IDictionary<string, string> headers, ContentStream responseStream);
}