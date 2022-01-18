using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Common.Extensions;

namespace Genbox.SimpleS3.Extensions.BackBlazeB2
{
    public class BackBlazeB2Config : SimpleS3Config
    {
        private readonly IRegionConverter _converter = new RegionConverter(BackblazeB2RegionData.Instance);
        private BackBlazeB2Region _region;

        public BackBlazeB2Config()
        {
            ProviderName = "BackBlazeB2";
            EndpointTemplate = "{Scheme}://{Bucket}.s3.{Region}.backblazeb2.com";
        }

        public BackBlazeB2Config(string keyId, string secretKey, BackBlazeB2Region region) : this(new StringAccessKey(keyId, secretKey), region) { }

        public BackBlazeB2Config(IAccessKey credentials, BackBlazeB2Region region) : this()
        {
            Credentials = credentials;
            Region = region;
        }

        public BackBlazeB2Region Region
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