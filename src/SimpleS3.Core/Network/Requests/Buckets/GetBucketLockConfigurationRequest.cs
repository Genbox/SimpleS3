using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>
/// Gets the Object Lock configuration for a bucket. The rule specified in the Object Lock configuration will be applied by default to every new
/// object placed in the specified bucket.
/// </summary>
public class GetBucketLockConfigurationRequest : BaseRequest, IHasBucketName
{
    internal GetBucketLockConfigurationRequest() : base(HttpMethodType.GET) { }

    public GetBucketLockConfigurationRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }

    public string BucketName { get; set; }

    internal void Initialize(string bucketName)
    {
        BucketName = bucketName;
    }
}