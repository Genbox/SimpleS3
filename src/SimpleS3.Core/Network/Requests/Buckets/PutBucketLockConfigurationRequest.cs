using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Places an Object Lock configuration on the specified bucket. The rule specified in the Object Lock
/// configuration will be applied by default to every new object placed in the specified bucket.</summary>
public sealed class PutBucketLockConfigurationRequest : BaseRequest, IHasBucketName, IHasRequestPayer, IHasLock, IContentMd5Config, IAutoMapConfig
{
    internal PutBucketLockConfigurationRequest() : base(HttpMethodType.PUT) {}

    public PutBucketLockConfigurationRequest(string bucketName, bool enabled) : this()
    {
        Initialize(bucketName, enabled);
    }

    public bool Enabled { get; set; }
    public string? LockToken { get; set; }
    Func<Type, bool> IAutoMapConfig.AutoMapDisabledFor => x => x == typeof(IHasLock);
    public byte[]? ContentMd5 { get; set; }
    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;
    public string BucketName { get; set; }
    public LockMode LockMode { get; set; }
    public DateTimeOffset? LockRetainUntil { get; set; }
    public Payer RequestPayer { get; set; }

    internal void Initialize(string bucketName, bool enabled)
    {
        BucketName = bucketName;
        Enabled = enabled;
    }

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