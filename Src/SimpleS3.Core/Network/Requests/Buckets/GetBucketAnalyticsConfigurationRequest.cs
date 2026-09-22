using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Gets an analytics configuration.</summary>
public sealed class GetBucketAnalyticsConfigurationRequest : BaseRequest, IHasBucketName
{
    internal GetBucketAnalyticsConfigurationRequest() : base(HttpMethodType.GET) {}

    /// <summary>Creates a request.</summary>
    public GetBucketAnalyticsConfigurationRequest(string bucketName, string configurationId) : this()
    {
        Initialize(bucketName, configurationId);
    }

    /// <summary>Configuration identifier.</summary>
    public string ConfigurationId { get; set; } = null!;

    /// <summary>Bucket name.</summary>
    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName, string configurationId)
    {
        BucketName = bucketName;
        ConfigurationId = configurationId;
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        ConfigurationId = null!;
        BucketName = null!;
        base.Reset();
    }
}