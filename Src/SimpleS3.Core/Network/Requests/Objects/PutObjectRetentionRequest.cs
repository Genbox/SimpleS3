using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>Provides the <c>PutObjectRetentionRequest</c> API.</summary>
public sealed class PutObjectRetentionRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer, IHasBypassGovernanceRetention, IContentMd5Config
{
    internal PutObjectRetentionRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Provides the <c>PutObjectRetentionRequest</c> API.</summary>
    public PutObjectRetentionRequest(string bucketName, string objectKey, LockMode lockMode, DateTimeOffset retainUntil) : this()
    {
        Initialize(bucketName, objectKey, lockMode, retainUntil);
    }

    /// <summary>Provides the <c>LockMode</c> API.</summary>
    public LockMode LockMode { get; set; }

    /// <summary>Provides the <c>LockRetainUntil</c> API.</summary>
    public DateTimeOffset? LockRetainUntil { get; set; }

    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }

    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Provides the <c>BypassGovernanceRetention</c> API.</summary>
    public bool? BypassGovernanceRetention { get; set; }

    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; set; } = null!;

    /// <summary>Provides the <c>RequestPayer</c> API.</summary>
    public Payer RequestPayer { get; set; }

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; set; }

    internal void Initialize(string bucketName, string objectKey, LockMode lockMode, DateTimeOffset retainUntil)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
        LockMode = lockMode;
        LockRetainUntil = retainUntil;
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        BypassGovernanceRetention = null;
        ContentMd5 = null;
        LockMode = LockMode.Unknown;
        LockRetainUntil = null;
        RequestPayer = Payer.Unknown;
        VersionId = null;
        base.Reset();
    }
}