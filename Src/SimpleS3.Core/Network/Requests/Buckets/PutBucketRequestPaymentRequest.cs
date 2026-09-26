using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Provides the <c>PutBucketRequestPaymentRequest</c> API.</summary>
public sealed class PutBucketRequestPaymentRequest : BaseRequest, IHasBucketName, IContentMd5Config
{
    internal PutBucketRequestPaymentRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Provides the <c>PutBucketRequestPaymentRequest</c> API.</summary>
    public PutBucketRequestPaymentRequest(string bucketName, RequestPaymentPayer payer) : this()
    {
        Initialize(bucketName, payer);
    }

    /// <summary>Provides the <c>Payer</c> API.</summary>
    public RequestPaymentPayer Payer { get; set; }

    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }

    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    internal void Initialize(string bucketName, RequestPaymentPayer payer)
    {
        BucketName = bucketName;
        Payer = payer;
    }

    internal override void Reset()
    {
        ContentMd5 = null;
        Payer = RequestPaymentPayer.Unknown;
        BucketName = null!;
        base.Reset();
    }
}