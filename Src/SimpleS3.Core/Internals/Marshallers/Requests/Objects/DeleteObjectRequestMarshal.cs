using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects;

internal sealed class DeleteObjectRequestMarshal : IRequestMarshal<DeleteObjectRequest>
{
    public Stream? MarshalRequest(DeleteObjectRequest request, SimpleS3Config config)
    {
        if ((string.Equals(config.ProviderName, "GoogleCloudStorage", StringComparison.OrdinalIgnoreCase) || string.Equals(config.ProviderName, "Wasabi", StringComparison.OrdinalIgnoreCase)) && request.IfMatch != null)
            throw new NotSupportedException($"{config.ProviderName} does not enforce DeleteObject conditions. The delete was not sent.");

        request.SetOptionalHeader(HttpHeaders.IfMatch, request.IfMatch);
        return null;
    }
}