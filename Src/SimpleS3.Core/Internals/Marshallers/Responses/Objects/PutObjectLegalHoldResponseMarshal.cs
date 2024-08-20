using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;

internal sealed class PutObjectLegalHoldResponseMarshal : IResponseMarshal<PutObjectLegalHoldResponse>
{
    public void MarshalResponse(SimpleS3Config config, PutObjectLegalHoldResponse response, IDictionary<string, string> headers, Stream responseStream)
    {
        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);
    }
}