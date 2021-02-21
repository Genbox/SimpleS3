using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Authentication;

namespace Genbox.SimpleS3.Extensions.BackBlazeB2
{
    public class B2Config : Config
    {
        private readonly IRegionConverter _converter = new RegionConverter(new B2RegionData());
        private B2Region _region;

        public B2Config()
        {
            ProviderType = "BackBlazeB2";
        }

        public B2Config(string keyId, string secretKey, B2Region region) : this(new StringAccessKey(keyId, secretKey), region) { }

        public B2Config(IAccessKey credentials, B2Region region) : this()
        {
            Credentials = credentials;
            Region = region;
        }

        public B2Region Region
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