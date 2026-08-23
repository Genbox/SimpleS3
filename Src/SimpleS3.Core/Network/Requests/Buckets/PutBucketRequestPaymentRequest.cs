using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

public sealed class PutBucketRequestPaymentRequest : BaseRequest, IHasBucketName, IHasExpectedBucketOwner, IContentMd5Config
{
    internal PutBucketRequestPaymentRequest() : base(HttpMethodType.PUT) {}
    public PutBucketRequestPaymentRequest(string bucketName, RequestPaymentPayer payer) : this() => Initialize(bucketName, payer);
    public string BucketName { get; set; } = null!;
    public byte[]? ContentMd5 { get; set; }
    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;
    public string? ExpectedBucketOwner { get; set; }
    public RequestPaymentPayer Payer { get; set; }
    internal void Initialize(string bucketName, RequestPaymentPayer payer) { BucketName = bucketName; Payer = payer; }
    public override void Reset() { ContentMd5 = null; ExpectedBucketOwner = null; Payer = RequestPaymentPayer.Unknown; base.Reset(); }
}