using System;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>Sets an object's current Legal Hold status.</summary>
public sealed class PutObjectLegalHoldRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer, IContentMd5Config, IHasLegalHold
{
    internal PutObjectLegalHoldRequest() : base(HttpMethodType.PUT) { }

    public PutObjectLegalHoldRequest(string bucketName, string objectKey, bool legalHold) : this()
    {
        Initialize(bucketName, objectKey, legalHold);
    }

    public byte[]? ContentMd5 { get; set; }

    Func<bool> IContentMd5Config.ForceContentMd5 => () => true;

    public string BucketName { get; set; }
    public bool? LockLegalHold { get; set; }
    public string ObjectKey { get; set; }
    public Payer RequestPayer { get; set; }
    public string? VersionId { get; set; }

    internal void Initialize(string bucketName, string objectKey, bool legalHold)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
        LockLegalHold = legalHold;
    }

    public override void Reset()
    {
        RequestPayer = Payer.Unknown;
        VersionId = null;
        ContentMd5 = null;
        LockLegalHold = null;

        base.Reset();
    }
}