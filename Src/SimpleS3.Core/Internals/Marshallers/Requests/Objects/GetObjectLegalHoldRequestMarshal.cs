using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects;

internal sealed class GetObjectLegalHoldRequestMarshal : IRequestMarshal<GetObjectLegalHoldRequest>
{
    public Stream? MarshalRequest(GetObjectLegalHoldRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.LegalHold, string.Empty);
        return null;
    }
}