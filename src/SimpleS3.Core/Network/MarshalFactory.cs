using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Abstracts.Marshallers;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Marshallers.Requests;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
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

        public Stream MarshalRequest<T>(T request, IConfig config) where T : IRequest
        {
            //Auto map common properties
            GenericRequestMapper.Map(request);

            Stream content = null;

            //Map specific properties. Get the content if there is any.
            if (_requestMarshals.TryGetValue(typeof(T), out IRequestMarshal marshaller))
                content = ((IRequestMarshal<T>)marshaller).MarshalRequest(request, config);

            //If the specific mapper did not return a content, but the request has content, then map it here
            if (content == null && request is IHasContent hasContent)
                content = hasContent.Content;

            if (request is IHasContentMd5 hasContentMd5)
            {
                if (config.AlwaysCalculateContentMd5 || request is IContentMd5Config md5Config && md5Config.ForceContentMd5())
                {
                    string md5Hash = content == null ? "1B2M2Y8AsgTpgAmY7PhCfg==" : Convert.ToBase64String(CryptoHelper.Md5Hash(content, true));
                    request.SetHeader(HttpHeaders.ContentMd5, md5Hash);
                }
                else if (hasContentMd5.ContentMd5 != null)
                    request.SetHeader(HttpHeaders.ContentMd5, hasContentMd5.ContentMd5, BinaryEncoding.Base64);
            }

            return content;
        }

        public void MarshalResponse<TRequest, TResponse>(IConfig config, TRequest request, TResponse response, IDictionary<string, string> headers, Stream responseStream) where TRequest : IRequest where TResponse : IResponse
        {
            if (_responseMarshals.TryGetValue(typeof(TResponse), out IResponseMarshal marshaller))
                ((IResponseMarshal<TRequest, TResponse>)marshaller).MarshalResponse(config, request, response, headers, responseStream);
        }
    }
}