using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;

namespace Genbox.SimpleS3.Core.Internals.Network;

internal sealed class PostMapperFactory : IPostMapperFactory
{
    private readonly Dictionary<string, IPostMapper> _postMappers;

    public PostMapperFactory(IEnumerable<IPostMapper> postMappers)
    {
        _postMappers = postMappers.ToDictionary(x =>
        {
            Type type = x.GetType();
            Type iType = type.GetInterfaces()[0];
            Type[] args = iType.GetGenericArguments();

            return $"{args[0].Name}-{args[1].Name}";
        }, x => x, StringComparer.Ordinal);
    }

    public void PostMap<TRequest, TResponse>(SimpleS3Config config, TRequest request, TResponse response) where TRequest : IRequest where TResponse : IResponse
    {
        if (_postMappers.TryGetValue($"{typeof(TRequest).Name}-{typeof(TResponse).Name}", out IPostMapper? marshaller))
            ((IPostMapper<TRequest, TResponse>)marshaller).PostMap(request, response);
    }
}