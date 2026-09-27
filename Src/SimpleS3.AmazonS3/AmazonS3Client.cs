using Genbox.SimpleS3.Core;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Extensions.AmazonS3;
using Genbox.SimpleS3.ProviderBase;

namespace Genbox.SimpleS3.AmazonS3;

/// <summary>This class provides a convenient way to access all the functionality related to the S3 service, buckets and objects at the same time.</summary>
public sealed class AmazonS3Client : SimpleClient, IDisposable
{
    private readonly ProviderClientOwner? _owner;

    /// <summary>Creates a new instance of <see cref="AmazonS3Client" /></summary>
    /// <param name="keyId">The key id</param>
    /// <param name="accessKey">The secret access key</param>
    /// <param name="region">The region you wish to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public AmazonS3Client(string keyId, byte[] accessKey, AmazonS3Region region, NetworkConfig? networkConfig = null) : this(new AccessKey(keyId, accessKey), region, networkConfig) {}

    /// <summary>Creates a new instance of <see cref="AmazonS3Client" /></summary>
    /// <param name="keyId">The key id</param>
    /// <param name="accessKey">The secret access key</param>
    /// <param name="region">The region you wish to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public AmazonS3Client(string keyId, string accessKey, AmazonS3Region region, NetworkConfig? networkConfig = null) : this(new StringAccessKey(keyId, accessKey), region, networkConfig) {}

    /// <summary>Creates a client using temporary AWS session credentials.</summary>
    /// <param name="keyId">The access key id.</param>
    /// <param name="accessKey">The secret access key.</param>
    /// <param name="sessionToken">The AWS session token.</param>
    /// <param name="region">The region you wish to use.</param>
    /// <param name="networkConfig">Network configuration.</param>
    public AmazonS3Client(string keyId, string accessKey, string sessionToken, AmazonS3Region region, NetworkConfig? networkConfig = null) : this(new StringSessionAccessKey(keyId, accessKey, sessionToken), region, networkConfig) {}

    /// <summary>Creates a new instance of <see cref="AmazonS3Client" /></summary>
    /// <param name="credentials">The credentials to use</param>
    /// <param name="region">The region you wish to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public AmazonS3Client(IAccessKey? credentials, AmazonS3Region region, NetworkConfig? networkConfig = null) : this(new AmazonS3Config(credentials, region), networkConfig) {}

    /// <summary>Creates a new instance of <see cref="AmazonS3Client" /></summary>
    /// <param name="config">The configuration you want to use</param>
    /// <param name="networkConfig">Network configuration</param>
    public AmazonS3Client(AmazonS3Config config, NetworkConfig? networkConfig = null) : this(new ProviderClientOwner(new AmazonS3InputValidator(), config, networkConfig)) {}

    /// <summary>Provides the <c>AmazonS3Client</c> API.</summary>
    public AmazonS3Client(AmazonS3Config config, INetworkDriver networkDriver) : this(new ProviderClientOwner(new AmazonS3InputValidator(), config, networkDriver: networkDriver)) {}

    private AmazonS3Client(ProviderClientOwner owner) : this(owner.Client)
    {
        _owner = owner;
    }

    internal AmazonS3Client(SimpleClient client) : base(client) {}

    /// <summary>Releases the services owned by this client.</summary>
    public void Dispose() => _owner?.Dispose();
}