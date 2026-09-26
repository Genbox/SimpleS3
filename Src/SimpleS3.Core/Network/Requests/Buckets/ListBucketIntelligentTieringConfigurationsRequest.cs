using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Request to list Intelligent-Tiering configurations.</summary>
public sealed class ListBucketIntelligentTieringConfigurationsRequest : BaseRequest, IHasBucketName
{
    internal ListBucketIntelligentTieringConfigurationsRequest() : base(HttpMethodType.GET) {}

    /// <summary>Creates a request.</summary>
    public ListBucketIntelligentTieringConfigurationsRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }

    /// <summary>Token from a previous truncated response.</summary>
    public string? ContinuationToken { get; set; }

    /// <summary>Bucket name.</summary>
    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName) => BucketName = bucketName;

    /// <summary>Provides the <c>Reset</c> API.</summary>
    internal override void Reset()
    {
        ContinuationToken = null;
        BucketName = null!;
        base.Reset();
    }
}