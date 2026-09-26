using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Provides the <c>GetBucketPolicyRequest</c> API.</summary>
public class GetBucketPolicyRequest : BaseRequest, IHasBucketName
{
    internal GetBucketPolicyRequest() : base(HttpMethodType.GET) {}

    /// <summary>Provides the <c>GetBucketPolicyRequest</c> API.</summary>
    public GetBucketPolicyRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!; // Initialized before a pooled request is used.

    internal void Initialize(string bucketName) => BucketName = bucketName;

    internal override void Reset()
    {
        BucketName = null!;
        base.Reset();
    }
}