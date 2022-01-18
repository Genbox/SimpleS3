using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Request;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IRequestMarshal { }

public interface IRequestMarshal<in T> : IRequestMarshal where T : IRequest
{
    Stream? MarshalRequest(T request, SimpleS3Config config);
}