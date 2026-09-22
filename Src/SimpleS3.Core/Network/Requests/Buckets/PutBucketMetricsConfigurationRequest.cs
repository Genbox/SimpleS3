using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Request to create or replace a bucket metrics configuration.</summary>
public sealed class PutBucketMetricsConfigurationRequest : BaseRequest, IHasBucketName, IHasContentMd5
{
    internal PutBucketMetricsConfigurationRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Creates a request.</summary>
    public PutBucketMetricsConfigurationRequest(string bucketName, string configurationId, S3MetricsConfiguration configuration) : this()
    {
        Initialize(bucketName, configurationId, configuration);
    }

    /// <summary>Configuration identifier.</summary>
    public string ConfigurationId { get; set; } = null!;

    /// <summary>Metrics configuration.</summary>
    public S3MetricsConfiguration Configuration { get; private set; } = null!;

    /// <summary>Bucket name.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>MD5 checksum of the request body.</summary>
    public byte[]? ContentMd5 { get; set; }

    internal void Initialize(string bucketName, string configurationId, S3MetricsConfiguration configuration)
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