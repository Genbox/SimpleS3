using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>
/// Places an Object Lock configuration on the specified bucket. The rule specified in the Object Lock configuration will be applied by default to every new object placed in
/// the specified bucket.
/// </summary>
public sealed class PutBucketLockConfigurationRequest : BaseRequest, IHasBucketName, IHasRequestPayer, IHasLock, IContentMd5Config, IAutoMapConfig
{
    internal PutBucketLockConfigurationRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Provides the <c>PutBucketLockConfigurationRequest</c> API.</summary>
    public PutBucketLockConfigurationRequest(string bucketName, bool enabled) : this()
    {
        Initialize(bucketName, enabled);
    }

    /// <summary>Provides the <c>Enabled</c> API.</summary>
    public bool Enabled { get; set; }
    /// <summary>Provides the <c>LockToken</c> API.</summary>
    public string? LockToken { get; set; }
    Func<Type, bool> IAutoMapConfig.AutoMapDisabledFor => x => x == typeof(IHasLock);
    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }
    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;
    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;
    /// <summary>Provides the <c>LockMode</c> API.</summary>
    public LockMode LockMode { get; set; }
    /// <summary>Provides the <c>LockRetainUntil</c> API.</summary>
    public DateTimeOffset? LockRetainUntil { get; set; }
    /// <summary>Provides the <c>RequestPayer</c> API.</summary>
    public Payer RequestPayer { get; set; }

    internal void Initialize(string bucketName, bool enabled)
    {
        BucketName = bucketName;
        Enabled = enabled;
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        LockToken = null;
        LockMode = LockMode.Unknown;
        LockRetainUntil = null;
        ContentMd5 = null;
        RequestPayer = Payer.Unknown;

        base.Reset();
    }
}