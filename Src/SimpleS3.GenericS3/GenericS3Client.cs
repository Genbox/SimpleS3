using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Extensions.GenericS3;
using Genbox.SimpleS3.ProviderBase;

namespace Genbox.SimpleS3.GenericS3;

/// <summary>This class provides a convenient way to access all the functionality related to the S3 service, buckets and objects at the same time.</summary>
public sealed class GenericS3Client : SimpleClient, IDisposable
{
    private readonly ProviderClientOwner? _owner;

    /// <summary>Creates a new instance of <see cref="GenericS3Client" /></summary>
    /// <param name="keyId">The key id</param>
    /// <param name="accessKey">The secret access key</param>
    /// <param name="endpoint">The endpoint you wish to use</param>
    /// <param name="regionCode">The region code to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public GenericS3Client(string keyId, byte[] accessKey, string endpoint, string regionCode, NetworkConfig? networkConfig = null) : this(new AccessKey(keyId, accessKey), endpoint, regionCode, networkConfig) {}

    /// <summary>Creates a new instance of <see cref="GenericS3Client" /></summary>
    /// <param name="keyId">The key id</param>
    /// <param name="accessKey">The secret access key</param>
    /// <param name="endpoint">The endpoint you wish to use</param>
    /// <param name="regionCode">The region code to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public GenericS3Client(string keyId, string accessKey, string endpoint, string regionCode, NetworkConfig? networkConfig = null) : this(new StringAccessKey(keyId, accessKey), endpoint, regionCode, networkConfig) {}

    /// <summary>Creates a new instance of <see cref="GenericS3Client" /></summary>
    /// <param name="credentials">The credentials to use</param>
    /// <param name="endpoint">The endpoint you wish to use</param>
    /// <param name="regionCode">The region code to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public GenericS3Client(IAccessKey? credentials, string endpoint, string regionCode, NetworkConfig? networkConfig = null) : this(new GenericS3Config(credentials, endpoint, regionCode), networkConfig) {}

    /// <summary>Creates a new instance of <see cref="GenericS3Client" /></summary>
    /// <param name="config">The configuration you want to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public GenericS3Client(GenericS3Config config, NetworkConfig? networkConfig = null) : this(new ProviderClientOwner(new GenericS3InputValidator(), config, networkConfig)) {}

    /// <summary>Provides the <c>GenericS3Client</c> API.</summary>
    public GenericS3Client(GenericS3Config config, INetworkDriver networkDriver) : this(new ProviderClientOwner(new GenericS3InputValidator(), config, networkDriver: networkDriver)) {}

    private GenericS3Client(ProviderClientOwner owner) : this(owner.Client)
    {
        _owner = owner;
    }

    internal GenericS3Client(SimpleClient client) : base(client) {}

    /// <summary>Releases the services owned by this client.</summary>
    public void Dispose() => _owner?.Dispose();
}