using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Common.Authentication;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Extensions.GenericS3;

/// <summary>Provides the <c>GenericS3Config</c> API.</summary>
[PublicAPI]
public class GenericS3Config() : SimpleS3Config("GenericS3", null!) // The endpoint is populated during binding and validated before use.
{
    /// <summary>Provides the <c>GenericS3Config</c> API.</summary>
    public GenericS3Config(string endpoint) : this()
    {
        Endpoint = endpoint;
    }

    /// <summary>Provides the <c>GenericS3Config</c> API.</summary>
    public GenericS3Config(IAccessKey? credentials, string endpoint, string regionCode) : this(endpoint)
    {
        Credentials = credentials;
        RegionCode = regionCode;
    }

    /// <summary>Provides the <c>GenericS3Config</c> API.</summary>
    public GenericS3Config(string keyId, string secretKey, string endpoint, string regionCode) : this(new StringAccessKey(keyId, secretKey), endpoint, regionCode) {}
}