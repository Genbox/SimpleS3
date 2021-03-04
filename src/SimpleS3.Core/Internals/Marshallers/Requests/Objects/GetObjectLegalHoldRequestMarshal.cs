using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects
{
    [UsedImplicitly]
    internal class GetObjectLegalHoldRequestMarshal : IRequestMarshal<GetObjectLegalHoldRequest>
    {
        public Stream? MarshalRequest(GetObjectLegalHoldRequest request, Config config)
        {
            request.SetQueryParameter(AmzParameters.LegalHold, string.Empty);
            return null;
        }
    }
}