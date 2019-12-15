using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects
{
    public class PutObjectAclResponse : BaseResponse, IHasRequestCharged
    {
        public bool RequestCharged { get; internal set; }
    }
}