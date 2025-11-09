using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Response;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IResponseMarshal;

public interface IResponseMarshal<in TResponse> : IResponseMarshal where TResponse : IResponse
{
    void MarshalResponse(SimpleS3Config config, TResponse response, IDictionary<string, string> headers, ContentStream responseStream);
}