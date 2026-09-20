using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>Gets an object's current Legal Hold status.</summary>
public class GetObjectLegalHoldRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer
{
    internal GetObjectLegalHoldRequest() : base(HttpMethodType.GET) {}

    /// <summary>Provides the <c>GetObjectLegalHoldRequest</c> API.</summary>
    public GetObjectLegalHoldRequest(string bucketName, string objectKey) : this()
    {
        Initialize(bucketName, objectKey);
    }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

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
        RequestPayer = Payer.Unknown;
        VersionId = null;

        base.Reset();
    }
}