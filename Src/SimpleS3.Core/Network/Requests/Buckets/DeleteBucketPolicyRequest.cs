using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Provides the <c>DeleteBucketPolicyRequest</c> API.</summary>
public class DeleteBucketPolicyRequest : BaseRequest, IHasBucketName
{
    internal DeleteBucketPolicyRequest() : base(HttpMethodType.DELETE) {}

    /// <summary>Provides the <c>DeleteBucketPolicyRequest</c> API.</summary>
    public DeleteBucketPolicyRequest(string bucketName) : this()
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