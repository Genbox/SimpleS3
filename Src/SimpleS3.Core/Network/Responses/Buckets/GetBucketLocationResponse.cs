namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

public sealed class GetBucketLocationResponse : BaseResponse
{
    public string? Location { get; internal set; }
}