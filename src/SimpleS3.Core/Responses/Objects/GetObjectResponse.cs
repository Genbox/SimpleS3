using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Responses.Objects.Properties;

namespace Genbox.SimpleS3.Core.Responses.Objects
{
    public class GetObjectResponse : HeadObjectResponse, IHasContent
    {
        public ContentReader Content { get; internal set; }
    }
}