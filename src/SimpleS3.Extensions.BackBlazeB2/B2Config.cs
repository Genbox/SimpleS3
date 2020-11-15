using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Common;

namespace Genbox.SimpleS3.Extensions.BackBlazeB2
{
    public class B2Config : Config
    {
        private B2Region _region;
        private readonly IRegionConverter _converter = new RegionConverter(new B2RegionData());

        public B2Config()
        {
            ProviderType = "BackBlazeB2";
        }

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