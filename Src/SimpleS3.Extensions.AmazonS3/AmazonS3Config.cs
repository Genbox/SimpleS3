﻿using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Common.Extensions;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Extensions.AmazonS3;

[PublicAPI]
public class AmazonS3Config : SimpleS3Config
{
    private readonly IRegionConverter _converter = new RegionConverter(AmazonS3RegionData.Instance);
    private AmazonS3Region _region;

    public AmazonS3Config() : base("AmazonS3")
    {
        EndpointTemplate = "{Scheme}://{Bucket:.}s3.{Region:.}amazonaws.com";
    }

    public AmazonS3Config(string keyId, string secretKey, AmazonS3Region region) : this(new StringAccessKey(keyId, secretKey), region) {}

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