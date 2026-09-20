using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Common.Extensions;

namespace Genbox.SimpleS3.Extensions.Wasabi;

/// <summary>Provides the <c>WasabiConfig</c> API.</summary>
public class WasabiConfig() : SimpleS3Config("Wasabi", "{Scheme}://{Bucket:.}s3.{Region:.}wasabisys.com")
{
    private static readonly IRegionConverter _converter = new RegionConverter(WasabiRegionData.Instance);
    private WasabiRegion _region;

    /// <summary>Provides the <c>WasabiConfig</c> API.</summary>
    public WasabiConfig(IAccessKey? credentials, string regionCode) : this()
    {
        Credentials = credentials;
        RegionCode = regionCode;
    }

    /// <summary>Provides the <c>WasabiConfig</c> API.</summary>
    public WasabiConfig(string keyId, string secretKey, WasabiRegion region) : this(new StringAccessKey(keyId, secretKey), region) {}

    /// <summary>Provides the <c>WasabiConfig</c> API.</summary>
    public WasabiConfig(IAccessKey? credentials, WasabiRegion region) : this(credentials, _converter.GetRegion(region).Code) {}

    /// <summary>Gets the region matching RegionCode, or Unknown for an unrecognized code. Setting a known region updates RegionCode.</summary>
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