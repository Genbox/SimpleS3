using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class PutBucketLifecycleConfigurationResponseMarshal : IResponseMarshal<PutBucketLifecycleConfigurationResponse>
{
    public void MarshalResponse(SimpleS3Config config, PutBucketLifecycleConfigurationResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        if (headers.TryGetHeader("x-amz-transition-default-minimum-object-size", out string? minimumSize))
            response.TransitionDefaultMinimumObjectSize = Core.Enums.Enums.TransitionDefaultMinimumObjectSize.Parse(minimumSize, TransitionDefaultMinimumObjectSizeFormat.DisplayName);
    }
}