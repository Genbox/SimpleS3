using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;

internal sealed class GetObjectResponseMarshal : IResponseMarshal<GetObjectResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetObjectResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        HeadObjectResponseMarshal.MapHeaders(response, headers);
        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);
        response.Content = responseStream;
    }
}