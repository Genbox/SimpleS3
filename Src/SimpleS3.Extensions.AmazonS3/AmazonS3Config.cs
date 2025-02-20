using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Common.Extensions;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Extensions.AmazonS3;

[PublicAPI]
public class AmazonS3Config() : SimpleS3Config("AmazonS3", "{Scheme}://{Bucket:.}s3.{Region:.}amazonaws.com")
{
    private static readonly RegionConverter _converter = new RegionConverter(AmazonS3RegionData.Instance);
    private AmazonS3Region _region;

    public AmazonS3Config(IAccessKey? credentials, string regionCode) : this()
    {
        Credentials = credentials;
        RegionCode = regionCode;
    }

    public AmazonS3Config(string keyId, string secretKey, AmazonS3Region region) : this(new StringAccessKey(keyId, secretKey), region) {}
    public AmazonS3Config(IAccessKey? credentials, AmazonS3Region region) : this(credentials, _converter.GetRegion(region).Code) {}

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