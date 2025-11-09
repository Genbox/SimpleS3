using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Factories;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Marshallers.Requests;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Internals.Network;

internal sealed class MarshalFactory : IMarshalFactory
{
    private readonly Dictionary<Type, IRequestMarshal> _requestMarshals;
    private readonly Dictionary<Type, IResponseMarshal> _responseMarshals;

    public MarshalFactory(IEnumerable<IRequestMarshal> requestMarshals, IEnumerable<IResponseMarshal> responseMarshals)
    {
        _requestMarshals = requestMarshals.ToDictionary(x =>
        {
            Type type = x.GetType();
            Type[] args = type.GetTypeArguments();
            return args[0];
        }, x => x);

        _responseMarshals = responseMarshals.ToDictionary(x =>
        {
            Type type = x.GetType();
            Type[] args = type.GetTypeArguments();
            return args[0];
        }, x => x);
    }

    public Stream? MarshalRequest<TRequest>(SimpleS3Config config, TRequest request) where TRequest : IRequest
    {
        //Auto map common properties
        GenericRequestMapper.Map(request);

        Stream? content = null;

        //Map specific properties. Get the content if there is any.
        if (_requestMarshals.TryGetValue(typeof(TRequest), out IRequestMarshal? marshaller))
            content = ((IRequestMarshal<TRequest>)marshaller).MarshalRequest(request, config);

        //If the specific mapper did not return a content, but the request has content, then map it here
        if (content == null && request is IHasContent hasContent)
            content = hasContent.Content;

        if (request is IHasContentMd5 hasContentMd5)
        {
            if (config.AlwaysCalculateContentMd5 || (request is IContentMd5Config md5Config && md5Config.ForceContentMd5()))
            {
                string md5Hash = content == null ? "1B2M2Y8AsgTpgAmY7PhCfg==" : Convert.ToBase64String(CryptoHelper.Md5Hash(content, true));
                request.SetHeader(HttpHeaders.ContentMd5, md5Hash);
            }
            else if (hasContentMd5.ContentMd5 != null)
                request.SetHeader(HttpHeaders.ContentMd5, hasContentMd5.ContentMd5, BinaryEncoding.Base64);
        }

        return content;
    }

    public void MarshalResponse<TResponse>(SimpleS3Config config, TResponse response, IDictionary<string, string> headers, ContentStream responseStream) where TResponse : IResponse
    {
        if (_responseMarshals.TryGetValue(typeof(TResponse), out IResponseMarshal? marshaller))
            ((IResponseMarshal<TResponse>)marshaller).MarshalResponse(config, response, headers, responseStream);
    }
}