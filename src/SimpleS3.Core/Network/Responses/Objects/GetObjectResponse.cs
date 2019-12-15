using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects
{
    public class GetObjectResponse : HeadObjectResponse, IHasContent, IHasRequestCharged
    {
        public ContentReader Content { get; internal set; }
        public bool RequestCharged { get; internal set; }
    }
}