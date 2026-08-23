namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

public sealed class GetPublicAccessBlockResponse : BaseResponse
{
    public bool BlockPublicAcls { get; internal set; }
    public bool IgnorePublicAcls { get; internal set; }
    public bool BlockPublicPolicy { get; internal set; }
    public bool RestrictPublicBuckets { get; internal set; }
}