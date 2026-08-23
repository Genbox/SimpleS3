using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

public sealed class PutObjectRetentionRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer, IHasBypassGovernanceRetention, IContentMd5Config
{
    internal PutObjectRetentionRequest() : base(HttpMethodType.PUT) {}

    public PutObjectRetentionRequest(string bucketName, string objectKey, LockMode lockMode, DateTimeOffset retainUntil) : this()
    {
        Initialize(bucketName, objectKey, lockMode, retainUntil);
    }

    public bool? BypassGovernanceRetention { get; set; }
    public string BucketName { get; set; } = null!;
    public byte[]? ContentMd5 { get; set; }
    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;
    public LockMode LockMode { get; set; }
    public DateTimeOffset? LockRetainUntil { get; set; }
    public string ObjectKey { get; set; } = null!;
    public Payer RequestPayer { get; set; }
    public string? VersionId { get; set; }

    internal void Initialize(string bucketName, string objectKey, LockMode lockMode, DateTimeOffset retainUntil)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
        LockMode = lockMode;
        LockRetainUntil = retainUntil;
    }

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