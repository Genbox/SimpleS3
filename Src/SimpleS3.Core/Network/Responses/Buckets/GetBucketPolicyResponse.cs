namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

public class GetBucketPolicyResponse : BaseResponse
{
    public string Policy { get; internal set; }
}