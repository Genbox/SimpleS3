using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>
/// Returns the tag set associated with the bucket. To use this operation, you must have permission to perform the s3:GetBucketTagging action. By default, the bucket owner
/// has this permission and can grant this permission to others.
/// </summary>
public class GetBucketTaggingRequest : BaseRequest, IHasBucketName
{
    internal GetBucketTaggingRequest() : base(HttpMethodType.GET) {}

    /// <summary>Provides the <c>GetBucketTaggingRequest</c> API.</summary>
    public GetBucketTaggingRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName) => BucketName = bucketName;

    internal override void Reset()
    {
        BucketName = null!;
        base.Reset();
    }
}