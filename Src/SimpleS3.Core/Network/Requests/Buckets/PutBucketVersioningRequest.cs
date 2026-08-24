using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>Sets the versioning state of an existing bucket. To set the versioning state, you must be the bucket owner.</summary>
public class PutBucketVersioningRequest : BaseRequest, IHasBucketName, IContentMd5Config, IHasMfa
{
    internal PutBucketVersioningRequest() : base(HttpMethodType.PUT)
    {
        Mfa = new MfaAuthenticationBuilder();
    }

    /// <summary>Provides the <c>PutBucketVersioningRequest</c> API.</summary>
    public PutBucketVersioningRequest(string bucketName, bool enabled) : this()
    {
        Initialize(bucketName, enabled);
    }

    /// <summary>Provides the <c>Status</c> API.</summary>
    public bool Status { get; set; }
    /// <summary>Provides the <c>ForceContentMd5</c> API.</summary>
    public Func<bool> ForceContentMd5 => () => true;
    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }
    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;
    /// <summary>Provides the <c>Mfa</c> API.</summary>
    public MfaAuthenticationBuilder Mfa { get; }

    internal void Initialize(string bucketName, bool enabled)
    {
        BucketName = bucketName;
        Status = enabled;
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        ContentMd5 = null;
        Mfa.Reset();
        base.Reset();
    }
}