using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

public class PutBucketPolicyRequest : BaseRequest, IHasBucketName, IHasContentMd5
{
    internal PutBucketPolicyRequest() : base(HttpMethodType.PUT) {}

    public PutBucketPolicyRequest(string bucketName, string policy) : this()
    {
        Initialize(bucketName, policy);
    }

    public bool ConfirmRemoveSelfBucketAccess { get; set; }
    public byte[]? ContentMd5 { get; set; }
    public string Policy { get; set; }

    public string BucketName { get; set; }

    internal void Initialize(string bucketName, string policy)
    {
        BucketName = bucketName;
        Policy = policy;
    }
}