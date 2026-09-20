using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>Provides the <c>DeleteObjectAnnotationRequest</c> API.</summary>
public sealed class DeleteObjectAnnotationRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer, IHasBypassGovernanceRetention
{
    internal DeleteObjectAnnotationRequest() : base(HttpMethodType.DELETE) {}

    /// <summary>Provides the <c>DeleteObjectAnnotationRequest</c> API.</summary>
    public DeleteObjectAnnotationRequest(string bucketName, string objectKey, string annotationName) : this()
    {
        Initialize(bucketName, objectKey, annotationName);
    }

    /// <summary>Provides the <c>AnnotationName</c> API.</summary>
    public string AnnotationName { get; private set; } = null!;

    /// <summary>Provides the <c>ObjectIfMatch</c> API.</summary>
    public string? ObjectIfMatch { get; set; }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Provides the <c>BypassGovernanceRetention</c> API.</summary>
    public bool? BypassGovernanceRetention { get; set; }

    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; set; } = null!;

    /// <summary>Provides the <c>RequestPayer</c> API.</summary>
    public Payer RequestPayer { get; set; }

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; set; }

    internal void Initialize(string bucketName, string objectKey, string annotationName)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
        AnnotationName = annotationName;
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        AnnotationName = null!;
        VersionId = null;
        RequestPayer = Payer.Unknown;
        ObjectIfMatch = null;
        BypassGovernanceRetention = null;
        base.Reset();
    }
}