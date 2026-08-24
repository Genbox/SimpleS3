using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Provides the <c>DeleteBucketAnalyticsConfigurationRequest</c> API.</summary>
public sealed class DeleteBucketAnalyticsConfigurationRequest : BaseRequest, IHasBucketName, IHasExpectedBucketOwner
{
    internal DeleteBucketAnalyticsConfigurationRequest() : base(HttpMethodType.DELETE) {}
    /// <summary>Provides the <c>DeleteBucketAnalyticsConfigurationRequest</c> API.</summary>
    public DeleteBucketAnalyticsConfigurationRequest(string bucketName, string configurationId) : this() => Initialize(bucketName, configurationId);
    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;
    /// <summary>Provides the <c>ConfigurationId</c> API.</summary>
    public string ConfigurationId { get; set; } = null!;
    /// <summary>Provides the <c>ExpectedBucketOwner</c> API.</summary>
    public string? ExpectedBucketOwner { get; set; }
    internal void Initialize(string bucketName, string configurationId) { BucketName = bucketName; ConfigurationId = configurationId; }
    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset() { ExpectedBucketOwner = null; base.Reset(); }
}