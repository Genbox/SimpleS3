using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

public sealed class GetBucketEncryptionResponse : BaseResponse
{
    public IList<S3ServerSideEncryptionRule> Rules { get; } = new List<S3ServerSideEncryptionRule>();
}