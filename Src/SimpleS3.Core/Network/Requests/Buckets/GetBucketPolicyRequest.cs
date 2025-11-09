using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

public class GetBucketPolicyRequest : BaseRequest, IHasBucketName
{
    internal GetBucketPolicyRequest() : base(HttpMethodType.GET) {}

    public GetBucketPolicyRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }

    public string BucketName { get; set; }

    internal void Initialize(string bucketName)
    {
        BucketName = bucketName;
    }
}