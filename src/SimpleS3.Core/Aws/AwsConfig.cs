using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Aws
{
    [PublicAPI]
    public class AwsConfig : Config
    {
        private readonly IRegionConverter _converter = new RegionConverter(new AwsRegionData());
        private AwsRegion _region;

        public AwsConfig()
        {
            ProviderType = "AmazonS3";
        }

        public AwsConfig(IAccessKey credentials, AwsRegion region) : this()
        {
            Credentials = credentials;
            Region = region;
        }

        public AwsRegion Region
        {
            get => _region;
            set
            {
                _region = value;
                RegionCode = _converter.GetRegion(value);
            }
        }
    }
}