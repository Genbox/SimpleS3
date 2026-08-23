using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects;

internal sealed class ListObjectAnnotationsRequestMarshal : IRequestMarshal<ListObjectAnnotationsRequest>
{
    public Stream? MarshalRequest(ListObjectAnnotationsRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Annotation, string.Empty);
        request.SetOptionalQueryParameter(AmzParameters.AnnotationPrefix, request.AnnotationPrefix);
        request.SetOptionalQueryParameter(AmzParameters.ContinuationToken, request.ContinuationToken);
        request.SetQueryParameter(AmzParameters.MaxAnnotationResults, request.MaxAnnotationResults);
        return null;
    }
}