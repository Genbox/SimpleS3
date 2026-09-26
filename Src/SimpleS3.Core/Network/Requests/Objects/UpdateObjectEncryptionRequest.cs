using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>Atomically changes an encrypted object's SSE-KMS configuration.</summary>
public sealed class UpdateObjectEncryptionRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer, IContentMd5Config
{
    internal UpdateObjectEncryptionRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Provides the <c>UpdateObjectEncryptionRequest</c> API.</summary>
    public UpdateObjectEncryptionRequest(string bucketName, string objectKey, string kmsKeyArn) : this()
    {
        Initialize(bucketName, objectKey, kmsKeyArn);
    }

    /// <summary>The required customer-managed KMS key ARN.</summary>
    public string KmsKeyArn { get; private set; } = null!;

    /// <summary>Whether to enable S3 Bucket Keys.</summary>
    public bool? BucketKeyEnabled { get; set; }

    /// <summary>The MD5 digest of the request body.</summary>
    public byte[]? ContentMd5 { get; set; }

    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;

    /// <summary>The bucket name.</summary>
    public string BucketName { get; private set; } = null!;

    /// <summary>The object key.</summary>
    public string ObjectKey { get; private set; } = null!;

    /// <summary>The requester-pays setting.</summary>
    public Payer RequestPayer { get; set; }

    /// <summary>The object version ID.</summary>
    public string? VersionId { get; set; }

    internal void Initialize(string bucketName, string objectKey, string kmsKeyArn)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
        KmsKeyArn = kmsKeyArn;
    }

    internal override void Reset()
    {
        BucketKeyEnabled = null;
        ContentMd5 = null;
        VersionId = null;
        RequestPayer = Payer.Unknown;
        KmsKeyArn = null!;
        BucketName = null!;
        ObjectKey = null!;
        base.Reset();
    }
}