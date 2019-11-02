using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request.Object
{
    [UsedImplicitly]
    internal class DeleteObjectRequestMarshal : IRequestMarshal<DeleteObjectRequest>
    {
        public Stream MarshalRequest(DeleteObjectRequest request, IS3Config config)
        {
            request.AddHeader(AmzHeaders.XAmzMfa, request.Mfa);
            request.AddHeader(AmzHeaders.XAmzBypassGovernanceRetention, request.BypassGovernanceRetention);
            request.AddQueryParameter(AmzParameters.VersionId, request.VersionId);
            return null;
        }
    }
}