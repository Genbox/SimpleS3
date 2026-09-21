using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;

internal sealed class DeleteObjectAnnotationResponseMarshal : IResponseMarshal<DeleteObjectAnnotationResponse>
{
    public void MarshalResponse(SimpleS3Config config, DeleteObjectAnnotationResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        response.VersionId = headers.GetOptionalValue(AmzHeaders.XAmzObjectVersionId);
        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);
    }
}