using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Network.Responses.Properties;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects
{
    public class GetObjectResponse : HeadObjectResponse, IHasContent
    {
        public ContentReader Content { get; internal set; }
    }
}