using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects;

internal sealed class HeadObjectRequestMarshal : IRequestMarshal<HeadObjectRequest>
{
    public Stream? MarshalRequest(HeadObjectRequest request, SimpleS3Config config)
    {
        if (request.EnableChecksum)
            request.SetHeader(AmzHeaders.XAmzChecksumMode, "ENABLED");

        return null;
    }
}