using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Provides the <c>PutBucketPolicyRequest</c> API.</summary>
public class PutBucketPolicyRequest : BaseRequest, IHasBucketName, IHasContentMd5
{
    internal PutBucketPolicyRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Provides the <c>PutBucketPolicyRequest</c> API.</summary>
    public PutBucketPolicyRequest(string bucketName, string policy) : this()
    {
        Initialize(bucketName, policy);
    }

    /// <summary>Provides the <c>ConfirmRemoveSelfBucketAccess</c> API.</summary>
    public bool ConfirmRemoveSelfBucketAccess { get; set; }

    /// <summary>Provides the <c>Policy</c> API.</summary>
    public string Policy { get; set; } = null!; // Initialized before a pooled request is used.

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!; // Initialized before a pooled request is used.

    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }

    internal void Initialize(string bucketName, string policy)
    {
        BucketName = bucketName;
        Policy = policy;
    }

    internal override void Reset()
    {
        ConfirmRemoveSelfBucketAccess = false;
        ContentMd5 = null;

        Policy = null!;
        BucketName = null!;
        base.Reset();
    }
}