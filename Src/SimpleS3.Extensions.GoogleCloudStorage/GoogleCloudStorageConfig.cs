﻿using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Common.Extensions;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Extensions.GoogleCloudStorage;

[PublicAPI]
public class GoogleCloudStorageConfig : SimpleS3Config
{
    private static readonly IRegionConverter _converter = new RegionConverter(GoogleCloudStorageRegionData.Instance);
    private GoogleCloudStorageRegion _region;

    public GoogleCloudStorageConfig() : base("GoogleCloudStorage", "{Scheme}://{Bucket:.}storage.googleapis.com")
    {
        //Google does not support chunked streaming uploads
        PayloadSignatureMode = SignatureMode.FullSignature;
    }

    public GoogleCloudStorageConfig(IAccessKey? credentials, string regionCode) : this()
    {
        Credentials = credentials;
        RegionCode = regionCode;
    }

    public GoogleCloudStorageConfig(string keyId, string secretKey, GoogleCloudStorageRegion region) : this(new StringAccessKey(keyId, secretKey), region) {}

    public GoogleCloudStorageConfig(IAccessKey? credentials, GoogleCloudStorageRegion region) : this(credentials, _converter.GetRegion(region).Code) {}

    public GoogleCloudStorageRegion Region
    {
        get => _region;
        set
        {
            _region = value;
            RegionCode = _converter.GetRegion(value).Code;
        }
    }
}