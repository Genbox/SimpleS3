using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Provides the <c>DeleteBucketOwnershipControlsRequest</c> API.</summary>
public sealed class DeleteBucketOwnershipControlsRequest : BaseRequest, IHasBucketName
{
    internal DeleteBucketOwnershipControlsRequest() : base(HttpMethodType.DELETE) {}

    /// <summary>Provides the <c>DeleteBucketOwnershipControlsRequest</c> API.</summary>
    public DeleteBucketOwnershipControlsRequest(string bucketName) : this()
    {
        Initialize(bucketName);
    }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName) => BucketName = bucketName;

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        base.Reset();
    }
}