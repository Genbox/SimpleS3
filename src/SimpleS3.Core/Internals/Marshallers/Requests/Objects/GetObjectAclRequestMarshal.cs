using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects
{
    internal class GetObjectAclRequestMarshal : IRequestMarshal<GetObjectAclRequest>
    {
        public Stream? MarshalRequest(GetObjectAclRequest request, Config config)
        {
            request.SetQueryParameter(AmzParameters.Acl, string.Empty);
            return null;
        }
    }
}