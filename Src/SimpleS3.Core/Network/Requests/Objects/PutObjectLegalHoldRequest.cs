using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>Sets an object's current Legal Hold status.</summary>
public sealed class PutObjectLegalHoldRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer, IContentMd5Config, IHasLegalHold
{
    internal PutObjectLegalHoldRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Provides the <c>PutObjectLegalHoldRequest</c> API.</summary>
    public PutObjectLegalHoldRequest(string bucketName, string objectKey, bool legalHold) : this()
    {
        Initialize(bucketName, objectKey, legalHold);
    }

    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }

    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;
    /// <summary>Provides the <c>LockLegalHold</c> API.</summary>
    public bool? LockLegalHold { get; set; }
    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; set; } = null!;
    /// <summary>Provides the <c>RequestPayer</c> API.</summary>
    public Payer RequestPayer { get; set; }
    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; set; }

    internal void Initialize(string bucketName, string objectKey, bool legalHold)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
        LockLegalHold = legalHold;
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        RequestPayer = Payer.Unknown;
        VersionId = null;
        ContentMd5 = null;
        LockLegalHold = null;

        base.Reset();
    }
}