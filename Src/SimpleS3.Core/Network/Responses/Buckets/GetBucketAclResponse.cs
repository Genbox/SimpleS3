using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

public sealed class GetBucketAclResponse : BaseResponse
{
    public GetBucketAclResponse() => Grants = new List<S3Grant>();
    public S3Identity Owner { get; internal set; }
    public IList<S3Grant> Grants { get; }
}