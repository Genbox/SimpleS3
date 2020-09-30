using System;
using System.Collections.Generic;
using System.Linq;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Factories;

namespace Genbox.SimpleS3.Core.Network
{
    public class PostMapperFactory : IPostMapperFactory
    {
        private readonly IDictionary<string, IPostMapper> _postMappers;

        public PostMapperFactory(IEnumerable<IPostMapper> postMappers)
        {
            _postMappers = postMappers.ToDictionary(x =>
            {
                Type type = x.GetType();
                Type iType = type.GetInterfaces().First();
                Type[] args = iType.GetGenericArguments();

                return $"{args[0].Name}-{args[1].Name}";
            }, x => x);
        }

        public void PostMap<TRequest, TResponse>(IConfig config, TRequest request, TResponse response) where TRequest : IRequest where TResponse : IResponse
        {
            if (_postMappers.TryGetValue($"{typeof(TRequest).Name}-{typeof(TResponse).Name}", out IPostMapper marshaller))
                ((IPostMapper<TRequest, TResponse>)marshaller).PostMap(config, request, response);
        }
    }
}