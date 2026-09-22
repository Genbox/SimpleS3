using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Creates or replaces an inventory configuration.</summary>
public sealed class PutBucketInventoryConfigurationRequest : BaseRequest, IHasBucketName, IContentMd5Config
{
    internal PutBucketInventoryConfigurationRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Creates a request.</summary>
    public PutBucketInventoryConfigurationRequest(string bucketName, string configurationId, S3InventoryConfiguration configuration) : this()
    {
        Initialize(bucketName, configurationId, configuration);
    }

    /// <summary>Configuration identifier.</summary>
    public string ConfigurationId { get; set; } = null!;

    /// <summary>Inventory configuration.</summary>
    public S3InventoryConfiguration Configuration { get; private set; } = null!;

    /// <summary>MD5 checksum of the request body.</summary>
    public byte[]? ContentMd5 { get; set; }

    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;

    /// <summary>Bucket name.</summary>
    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName, string configurationId, S3InventoryConfiguration configuration)
    {
        BucketName = bucketName;
        ConfigurationId = configurationId;
        Configuration = configuration;
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        Configuration = null!;
        ContentMd5 = null;
        ConfigurationId = null!;
        BucketName = null!;
        base.Reset();
    }
}