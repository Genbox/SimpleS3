using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Authentication;

namespace Genbox.SimpleS3.Extensions.AwsS3
{
    public class AwsConfig : Config
    {
        private readonly IRegionConverter _converter = new RegionConverter(new AwsRegionData());
        private AwsRegion _region;

        public AwsConfig()
        {
            ProviderType = "AmazonS3";
        }

        public AwsConfig(string keyId, string secretKey, AwsRegion region) : this(new StringAccessKey(keyId, secretKey), region) { }

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