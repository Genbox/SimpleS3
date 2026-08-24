using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>GetBucketOwnershipControlsResponse</c> API.</summary>
public sealed class GetBucketOwnershipControlsResponse : BaseResponse
{
    /// <summary>Provides the <c>Rules</c> API.</summary>
    public IList<S3OwnershipControlsRule> Rules { get; } = new List<S3OwnershipControlsRule>();
}