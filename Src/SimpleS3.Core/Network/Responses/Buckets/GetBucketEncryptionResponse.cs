using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>GetBucketEncryptionResponse</c> API.</summary>
public sealed class GetBucketEncryptionResponse : BaseResponse
{
    /// <summary>Provides the <c>Rules</c> API.</summary>
    public IList<S3ServerSideEncryptionRule> Rules { get; } = new List<S3ServerSideEncryptionRule>();
}