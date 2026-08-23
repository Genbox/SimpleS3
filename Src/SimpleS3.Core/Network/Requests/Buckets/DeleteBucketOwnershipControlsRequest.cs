using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

public sealed class DeleteBucketOwnershipControlsRequest : BaseRequest, IHasBucketName, IHasExpectedBucketOwner
{
    internal DeleteBucketOwnershipControlsRequest() : base(HttpMethodType.DELETE) {}
    public DeleteBucketOwnershipControlsRequest(string bucketName) : this() => Initialize(bucketName);
    public string BucketName { get; set; } = null!;
    public string? ExpectedBucketOwner { get; set; }
    internal void Initialize(string bucketName) => BucketName = bucketName;
    public override void Reset() { ExpectedBucketOwner = null; base.Reset(); }
}