using System.Collections.Generic;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects
{
    public class GetObjectAclResponse : BaseResponse, IHasRequestCharged
    {
        public GetObjectAclResponse()
        {
            Grants = new List<S3Grant>();
        }

        public S3Identity Owner { get; internal set; }

        public IList<S3Grant> Grants { get; }
        public bool RequestCharged { get; internal set; }
    }
}