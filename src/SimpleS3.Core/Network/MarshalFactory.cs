using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Factories;
using Genbox.SimpleS3.Abstracts.Marshal;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network
{
    [PublicAPI]
    public class MarshalFactory : IMarshalFactory
    {
        private readonly IDictionary<Type, IRequestMarshal> _requestMarshals;
        private readonly IDictionary<Type, IResponseMarshal> _responseMarshals;

        public MarshalFactory(IEnumerable<IRequestMarshal> requestMarshals, IEnumerable<IResponseMarshal> responseMarshals)
        {
            _requestMarshals = requestMarshals.ToDictionary(x =>
            {
                Type type = x.GetType();
                Type iType = type.GetInterfaces().First();
                Type[] args = iType.GetGenericArguments();

                return args[0];
            }, x => x);

            _responseMarshals = responseMarshals.ToDictionary(x =>
            {
                Type type = x.GetType();
                Type iType = type.GetInterfaces().First();
                Type[] args = iType.GetGenericArguments();

                return args[1];
            }, x => x);
        }

        public Stream MarshalRequest<T>(T request, IS3Config config) where T : IRequest
        {
            if (_requestMarshals.TryGetValue(typeof(T), out IRequestMarshal marshaller))
                return ((IRequestMarshal<T>)marshaller).MarshalRequest(request, config);

            return null;
        }

        public void MarshalResponse<TRequest, TResponse>(TRequest request, TResponse response, IDictionary<string, string> headers, Stream responseStream) where TRequest : IRequest where TResponse : IResponse
        {
            if (_responseMarshals.TryGetValue(typeof(TResponse), out IResponseMarshal marshaller))
                ((IResponseMarshal<TRequest, TResponse>)marshaller).MarshalResponse(request, response, headers, responseStream);
        }
    }
}