using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Returns the tag set associated with the bucket. To use this operation, you must have permission to perform the
/// s3:GetBucketTagging action. By default, the bucket owner has this permission and can grant this permission to others.</summary>
public class GetBucketTaggingRequest : BaseRequest, IHasBucketName
{
    internal GetBucketTaggingRequest() : base(HttpMethodType.GET) {}

    public GetBucketTaggingRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }

    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName)
    {
        BucketName = bucketName;
    }
}