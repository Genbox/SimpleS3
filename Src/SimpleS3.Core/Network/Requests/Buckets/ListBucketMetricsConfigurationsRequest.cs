using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Request to list bucket metrics configurations.</summary>
public sealed class ListBucketMetricsConfigurationsRequest : BaseRequest, IHasBucketName
{
    internal ListBucketMetricsConfigurationsRequest() : base(HttpMethodType.GET) {}

    /// <summary>Creates a request.</summary>
    public ListBucketMetricsConfigurationsRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }

    /// <summary>Token from a previous truncated response.</summary>
    public string? ContinuationToken { get; set; }

    /// <summary>Bucket name.</summary>
    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName) => BucketName = bucketName;

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {

        ContinuationToken = null;
        BucketName = null!;
        base.Reset();
    }
}