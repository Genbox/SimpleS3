using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

public sealed class GetBucketRequestPaymentResponse : BaseResponse
{
    public RequestPaymentPayer Payer { get; internal set; }
}