using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects;

internal sealed class GetObjectAnnotationRequestMarshal : IRequestMarshal<GetObjectAnnotationRequest>
{
    public Stream? MarshalRequest(GetObjectAnnotationRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Annotation, string.Empty);
        request.SetQueryParameter(AmzParameters.AnnotationName, request.AnnotationName);
        if (request.EnableChecksum)
            request.SetHeader(AmzHeaders.XAmzChecksumMode, "ENABLED");
        return null;
    }
}