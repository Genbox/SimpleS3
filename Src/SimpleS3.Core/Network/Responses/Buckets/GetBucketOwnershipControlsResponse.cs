using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

public sealed class GetBucketOwnershipControlsResponse : BaseResponse
{
    public IList<S3OwnershipControlsRule> Rules { get; } = new List<S3OwnershipControlsRule>();
}