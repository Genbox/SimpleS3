using System.Collections.Generic;
using Genbox.SimpleS3.Core.Network.Responses.Properties;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects
{
    public class GetObjectAclResponse : BaseResponse, IHasRequestCharged
    {
        public bool RequestCharged { get; internal set; }

        public S3Identity Owner { get; internal set; }

        public IList<S3Grant> Grants { get; internal set; }
    }
}