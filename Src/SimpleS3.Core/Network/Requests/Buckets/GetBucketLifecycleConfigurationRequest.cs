using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Returns the lifecycle configuration information set on the bucket.</summary>
public class GetBucketLifecycleConfigurationRequest : BaseRequest, IHasBucketName
{
    internal GetBucketLifecycleConfigurationRequest() : base(HttpMethodType.GET) {}

    /// <summary>Provides the <c>GetBucketLifecycleConfigurationRequest</c> API.</summary>
    public GetBucketLifecycleConfigurationRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName) => BucketName = bucketName;
}