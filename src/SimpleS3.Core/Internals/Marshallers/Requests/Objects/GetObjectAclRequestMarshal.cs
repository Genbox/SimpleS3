using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects
{
    [UsedImplicitly]
    internal class GetObjectAclRequestMarshal : IRequestMarshal<GetObjectAclRequest>
    {
        public Stream? MarshalRequest(GetObjectAclRequest request, IConfig config)
        {
            request.SetQueryParameter(AmzParameters.Acl, string.Empty);
            return null;
        }
    }
}