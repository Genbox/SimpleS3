using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Common.Extensions;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Extensions.AmazonS3;

/// <summary>Provides the <c>AmazonS3Config</c> API.</summary>
[PublicAPI]
public class AmazonS3Config() : SimpleS3Config("AmazonS3", "{Scheme}://{Bucket:.}s3.{Region:.}amazonaws.com")
{
    private static readonly RegionConverter _converter = new RegionConverter(AmazonS3RegionData.Instance);
    private AmazonS3Region _region;

    /// <summary>Provides the <c>AmazonS3Config</c> API.</summary>
    public AmazonS3Config(IAccessKey? credentials, string regionCode) : this()
    {
        Credentials = credentials;
        RegionCode = regionCode;
    }

    /// <summary>Provides the <c>AmazonS3Config</c> API.</summary>
    public AmazonS3Config(string keyId, string secretKey, AmazonS3Region region) : this(new StringAccessKey(keyId, secretKey), region) {}

    /// <summary>Creates a configuration using temporary AWS session credentials.</summary>
    public AmazonS3Config(string keyId, string secretKey, string sessionToken, AmazonS3Region region) : this(new StringSessionAccessKey(keyId, secretKey, sessionToken), region) {}

    /// <summary>Provides the <c>AmazonS3Config</c> API.</summary>
    public AmazonS3Config(IAccessKey? credentials, AmazonS3Region region) : this(credentials, _converter.GetRegion(region).Code) {}

    /// <summary>Gets the region matching RegionCode, or Unknown for an unrecognized code. Setting a known region updates RegionCode.</summary>
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