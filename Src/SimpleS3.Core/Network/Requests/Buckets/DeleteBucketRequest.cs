using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Deletes a bucket. All objects (including all object versions and delete markers) in the bucket must be deleted before the bucket itself can be deleted.</summary>
public class DeleteBucketRequest : BaseRequest, IHasBucketName
{
    internal DeleteBucketRequest() : base(HttpMethodType.DELETE) {}

    /// <summary>Provides the <c>DeleteBucketRequest</c> API.</summary>
    public DeleteBucketRequest(string bucketName) : this()
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