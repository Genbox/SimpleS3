using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Common.Authentication;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Extensions.GenericS3;

[PublicAPI]
public class GenericS3Config() : SimpleS3Config("GenericS3")
{
    public GenericS3Config(string keyId, string secretKey, string endpoint, string regionCode) : this(new StringAccessKey(keyId, secretKey), endpoint, regionCode) {}

    public GenericS3Config(IAccessKey credentials, string endpoint, string regionCode) : this()
    {
        Credentials = credentials;
        Endpoint = new Uri(endpoint);
        RegionCode = regionCode;
    }
}