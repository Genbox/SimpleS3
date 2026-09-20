using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Common.Extensions;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Extensions.BackBlazeB2;

/// <summary>Provides the <c>BackBlazeB2Config</c> API.</summary>
[PublicAPI]
public class BackBlazeB2Config() : SimpleS3Config("BackBlazeB2", "{Scheme}://{Bucket:.}s3.{Region:.}backblazeb2.com")
{
    private static readonly IRegionConverter _converter = new RegionConverter(BackblazeB2RegionData.Instance);
    private BackBlazeB2Region _region;

    /// <summary>Provides the <c>BackBlazeB2Config</c> API.</summary>
    public BackBlazeB2Config(IAccessKey? credentials, string regionCode) : this()
    {
        Credentials = credentials;
        RegionCode = regionCode;
    }

    /// <summary>Provides the <c>BackBlazeB2Config</c> API.</summary>
    public BackBlazeB2Config(string keyId, string secretKey, BackBlazeB2Region region) : this(new StringAccessKey(keyId, secretKey), region) {}

    /// <summary>Provides the <c>BackBlazeB2Config</c> API.</summary>
    public BackBlazeB2Config(IAccessKey? credentials, BackBlazeB2Region region) : this(credentials, _converter.GetRegion(region).Code) {}

    /// <summary>Gets the region matching RegionCode, or Unknown for an unrecognized code. Setting a known region updates RegionCode.</summary>
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