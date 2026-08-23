namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

public sealed class GetBucketPolicyStatusResponse : BaseResponse
{
    public bool IsPublic { get; internal set; }
}