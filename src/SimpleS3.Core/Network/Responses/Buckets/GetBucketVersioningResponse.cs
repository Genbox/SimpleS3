namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

public class GetBucketVersioningResponse : BaseResponse
{
    public bool Status { get; internal set; }
    public bool MfaDelete { get; internal set; }
}