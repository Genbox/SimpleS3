using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>GetBucketRequestPaymentResponse</c> API.</summary>
public sealed class GetBucketRequestPaymentResponse : BaseResponse
{
    /// <summary>Provides the <c>Payer</c> API.</summary>
    public RequestPaymentPayer Payer { get; internal set; }
}