using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>
/// The DELETE operation removes the null version (if there is one) of an object and inserts a delete marker, which becomes the current version of the object. If there isn't
/// a null version, Amazon S3 does not remove any objects.
/// </summary>
public class DeleteObjectRequest : BaseRequest, IHasVersionId, IHasRequestPayer, IHasBypassGovernanceRetention, IHasObjectKey, IHasBucketName, IHasMfa
{
    internal DeleteObjectRequest() : base(HttpMethodType.DELETE)
    {
        Mfa = new MfaAuthenticationBuilder();
    }

    /// <summary>Provides the <c>DeleteObjectRequest</c> API.</summary>
    public DeleteObjectRequest(string bucketName, string objectKey) : this()
    {
        Initialize(bucketName, objectKey);
    }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Provides the <c>BypassGovernanceRetention</c> API.</summary>
    public bool? BypassGovernanceRetention { get; set; }

    /// <summary>Provides the <c>Mfa</c> API.</summary>
    public MfaAuthenticationBuilder Mfa { get; internal set; }

    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; set; } = null!;

    /// <summary>Provides the <c>RequestPayer</c> API.</summary>
    public Payer RequestPayer { get; set; }

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; set; }

    internal void Initialize(string bucketName, string objectKey)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        Mfa.Reset();
        BypassGovernanceRetention = null;
        RequestPayer = Payer.Unknown;
        VersionId = null;

        base.Reset();
    }
}