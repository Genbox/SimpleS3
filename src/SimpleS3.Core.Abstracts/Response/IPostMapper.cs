using Genbox.SimpleS3.Core.Abstracts.Request;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Response
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
    public interface IPostMapper { }

    public interface IPostMapper<in TRequest, in TResponse> : IPostMapper where TRequest : IRequest where TResponse : IResponse
    {
        void PostMap(SimpleS3Config config, TRequest request, TResponse response);
    }
}