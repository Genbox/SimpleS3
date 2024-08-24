using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Common.Extensions;

namespace Genbox.SimpleS3.Extensions.Wasabi;

public class WasabiConfig : SimpleS3Config
{
    private readonly IRegionConverter _converter = new RegionConverter(WasabiRegionData.Instance);
    private WasabiRegion _region;

    public WasabiConfig() : base("Wasabi")
    {
        Endpoint = "{Scheme}://{Bucket:.}s3.{Region:.}wasabisys.com";
    }

    public WasabiConfig(string keyId, string secretKey, WasabiRegion region) : this(new StringAccessKey(keyId, secretKey), region) {}

    public WasabiConfig(IAccessKey credentials, WasabiRegion region) : this()
    {
        Credentials = credentials;
        Region = region;
    }

    public WasabiRegion Region
    {
        get => _region;
        set
        {
            _region = value;
            RegionCode = _converter.GetRegion(value).Code;
        }
    }
}