using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Returns the lifecycle configuration information set on the bucket.</summary>
public class GetBucketLifecycleConfigurationRequest : BaseRequest, IHasBucketName
{
    internal GetBucketLifecycleConfigurationRequest() : base(HttpMethodType.GET) {}

    public GetBucketLifecycleConfigurationRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }

    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName)
    {
        BucketName = bucketName;
    }
}