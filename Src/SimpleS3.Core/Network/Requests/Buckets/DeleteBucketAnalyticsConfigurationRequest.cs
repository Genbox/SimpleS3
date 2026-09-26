using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Provides the <c>DeleteBucketAnalyticsConfigurationRequest</c> API.</summary>
public sealed class DeleteBucketAnalyticsConfigurationRequest : BaseRequest, IHasBucketName
{
    internal DeleteBucketAnalyticsConfigurationRequest() : base(HttpMethodType.DELETE) {}

    /// <summary>Provides the <c>DeleteBucketAnalyticsConfigurationRequest</c> API.</summary>
    public DeleteBucketAnalyticsConfigurationRequest(string bucketName, string configurationId) : this()
    {
        Initialize(bucketName, configurationId);
    }

    /// <summary>Provides the <c>ConfigurationId</c> API.</summary>
    public string ConfigurationId { get; set; } = null!;

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName, string configurationId)
    {
        BucketName = bucketName;
        ConfigurationId = configurationId;
    }

    internal override void Reset()
    {
        ConfigurationId = null!;
        BucketName = null!;
        base.Reset();
    }
}