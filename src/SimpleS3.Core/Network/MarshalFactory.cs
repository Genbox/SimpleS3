using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Factories;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Core.Internal.Marshal.Request;
using Genbox.SimpleS3.Core.Network.Requests.Properties;
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
                if (config.AutoCalculateContentMd5 || request.ForceContentMd5 != null && request.ForceContentMd5())
                {
                    string md5Hash = content == null ? "1B2M2Y8AsgTpgAmY7PhCfg==" : Convert.ToBase64String(CryptoHelper.Md5Hash(content, true));
                    request.AddHeader(HttpHeaders.ContentMd5, md5Hash);
                }
                else if (hasContentMd5.ContentMd5 != null)
                    request.AddHeader(HttpHeaders.ContentMd5, hasContentMd5.ContentMd5, BinaryEncoding.Base64);
            }

            return content;
        }

        public void MarshalResponse<TRequest, TResponse>(IS3Config config, TRequest request, TResponse response, IDictionary<string, string> headers, Stream responseStream) where TRequest : IRequest where TResponse : IResponse
        {
            if (_responseMarshals.TryGetValue(typeof(TResponse), out IResponseMarshal marshaller))
                ((IResponseMarshal<TRequest, TResponse>)marshaller).MarshalResponse(config, request, response, headers, responseStream);
        }
    }
}