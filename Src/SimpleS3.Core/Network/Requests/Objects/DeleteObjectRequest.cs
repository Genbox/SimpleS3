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

    public DeleteObjectRequest(string bucketName, string objectKey) : this()
    {
        Initialize(bucketName, objectKey);
    }

    public string BucketName { get; set; } = null!;
    public bool? BypassGovernanceRetention { get; set; }
    public MfaAuthenticationBuilder Mfa { get; internal set; }
    public string ObjectKey { get; set; } = null!;
    public Payer RequestPayer { get; set; }
    public string? VersionId { get; set; }

    internal void Initialize(string bucketName, string objectKey)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
    }

    public override void Reset()
    {
        Mfa.Reset();
        BypassGovernanceRetention = null;
        RequestPayer = Payer.Unknown;
        VersionId = null;

        base.Reset();
    }
}