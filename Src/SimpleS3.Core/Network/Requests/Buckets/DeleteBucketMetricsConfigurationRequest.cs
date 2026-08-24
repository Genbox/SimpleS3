using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Provides the <c>DeleteBucketMetricsConfigurationRequest</c> API.</summary>
public sealed class DeleteBucketMetricsConfigurationRequest : BaseRequest, IHasBucketName, IHasExpectedBucketOwner
{
    internal DeleteBucketMetricsConfigurationRequest() : base(HttpMethodType.DELETE) {}
    /// <summary>Provides the <c>DeleteBucketMetricsConfigurationRequest</c> API.</summary>
    public DeleteBucketMetricsConfigurationRequest(string bucketName, string configurationId) : this() => Initialize(bucketName, configurationId);
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