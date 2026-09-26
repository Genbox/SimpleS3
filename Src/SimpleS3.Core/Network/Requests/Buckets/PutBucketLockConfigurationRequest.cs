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
public sealed class PutBucketLockConfigurationRequest : BaseRequest, IHasBucketName, IHasRequestPayer, IContentMd5Config
{
    internal PutBucketLockConfigurationRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Provides the <c>PutBucketLockConfigurationRequest</c> API.</summary>
    public PutBucketLockConfigurationRequest(string bucketName, bool enabled) : this()
    {
        Initialize(bucketName, enabled);
    }

    /// <summary>Must be true. Object Lock cannot be disabled; clearing the default retention rule is a separate operation.</summary>
    public bool Enabled { get; set; }

    /// <summary>Provides the <c>LockToken</c> API.</summary>
    public string? LockToken { get; set; }

    /// <summary>Provides the <c>LockMode</c> API.</summary>
    public LockMode LockMode { get; set; }

    /// <summary>Default retention duration in days. Specify either days or years.</summary>
    public int? LockRetainDays { get; set; }

    /// <summary>Default retention duration in calendar years. Specify either days or years.</summary>
    public int? LockRetainYears { get; set; }

    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }

    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Provides the <c>RequestPayer</c> API.</summary>
    public Payer RequestPayer { get; set; }

    internal void Initialize(string bucketName, bool enabled)
    {
        BucketName = bucketName;
        Enabled = enabled;
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    internal override void Reset()
    {
        LockToken = null;
        LockMode = LockMode.Unknown;
        LockRetainDays = null;
        LockRetainYears = null;
        ContentMd5 = null;
        RequestPayer = Payer.Unknown;
        ExpectedBucketOwner = null;
        Enabled = default;
        BucketName = null!;
        base.Reset();
    }
}