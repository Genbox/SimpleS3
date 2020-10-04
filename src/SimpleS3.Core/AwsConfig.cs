using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core
{
    [PublicAPI]
    public class AwsConfig : Config
    {
        public AwsConfig() { }

        public AwsConfig(IAccessKey credentials, AwsRegion region)
        {
            Credentials = credentials;
            Region = region;
        }

        public AwsRegion Region { get; set; }
    }
}