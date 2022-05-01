using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Region;
using Genbox.SimpleS3.Core.Common;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Common.Extensions;

namespace Genbox.SimpleS3.Extensions.GoogleCloudStorage;

public class GoogleCloudStorageConfig : SimpleS3Config
{
    private readonly IRegionConverter _converter = new RegionConverter(GoogleCloudStorageRegionData.Instance);
    private GoogleCloudStorageRegion _region;

    public GoogleCloudStorageConfig()
    {
        ProviderName = "GoogleCloudStorage";

        //Google does not support chunked streaming uploads
        PayloadSignatureMode = SignatureMode.FullSignature;
        EndpointTemplate = "{Scheme}://{Bucket:.}storage.googleapis.com";
    }

    public GoogleCloudStorageConfig(string keyId, string secretKey, GoogleCloudStorageRegion region) : this(new StringAccessKey(keyId, secretKey), region) {}

    public GoogleCloudStorageConfig(IAccessKey credentials, GoogleCloudStorageRegion region) : this()
    {
        Credentials = credentials;
        Region = region;
    }

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