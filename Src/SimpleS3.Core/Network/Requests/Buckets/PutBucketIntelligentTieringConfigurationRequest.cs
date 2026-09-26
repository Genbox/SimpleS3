using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Request to create or replace an Intelligent-Tiering configuration.</summary>
public sealed class PutBucketIntelligentTieringConfigurationRequest : BaseRequest, IHasBucketName, IHasContentMd5
{
    internal PutBucketIntelligentTieringConfigurationRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Creates a request.</summary>
    public PutBucketIntelligentTieringConfigurationRequest(string bucketName, string configurationId, S3IntelligentTieringConfiguration configuration) : this()
    {
        Initialize(bucketName, configurationId, configuration);
    }

    /// <summary>Configuration identifier.</summary>
    public string ConfigurationId { get; set; } = null!;

    /// <summary>Intelligent-Tiering configuration.</summary>
    public S3IntelligentTieringConfiguration Configuration { get; private set; } = null!;

    /// <summary>Bucket name.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>MD5 checksum of the request body.</summary>
    public byte[]? ContentMd5 { get; set; }

    internal void Initialize(string bucketName, string configurationId, S3IntelligentTieringConfiguration configuration)
    {
        BucketName = bucketName;
        ConfigurationId = configurationId;
        Configuration = configuration;
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    internal override void Reset()
    {
        Configuration = null!;
        ContentMd5 = null;
        ConfigurationId = null!;
        BucketName = null!;
        base.Reset();
    }
}