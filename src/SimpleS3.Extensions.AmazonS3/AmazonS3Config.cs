using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Common.Extensions;

namespace Genbox.SimpleS3.Extensions.AmazonS3
{
    public class AmazonS3Config : Config
    {
        private readonly IRegionConverter _converter = new RegionConverter(AmazonS3RegionData.Instance);
        private AmazonS3Region _region;

        public AmazonS3Config()
        {
            ProviderName = "AmazonS3";
        }

        public AmazonS3Config(string keyId, string secretKey, AmazonS3Region region) : this(new StringAccessKey(keyId, secretKey), region) { }

        public AmazonS3Config(IAccessKey credentials, AmazonS3Region region) : this()
        {
            Credentials = credentials;
            Region = region;
        }

        public AmazonS3Region Region
        {
            get => _region;
            set
            {
                _region = value;
                RegionCode = _converter.GetRegion(value).Code;
            }
        }
    }
}