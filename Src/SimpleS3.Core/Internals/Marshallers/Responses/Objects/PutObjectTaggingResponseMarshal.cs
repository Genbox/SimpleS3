using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;

internal sealed class PutObjectTaggingResponseMarshal : IResponseMarshal<PutObjectTaggingResponse>
{
    public void MarshalResponse(SimpleS3Config config, PutObjectTaggingResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);
        response.VersionId = headers.GetOptionalValue(AmzHeaders.XAmzVersionId);
    }
}