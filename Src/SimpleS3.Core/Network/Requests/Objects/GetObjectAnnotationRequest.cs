using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>Provides the <c>GetObjectAnnotationRequest</c> API.</summary>
public sealed class GetObjectAnnotationRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer
{
    internal GetObjectAnnotationRequest() : base(HttpMethodType.GET) {}

    /// <summary>Provides the <c>GetObjectAnnotationRequest</c> API.</summary>
    public GetObjectAnnotationRequest(string bucketName, string objectKey, string annotationName) : this()
    {
        Initialize(bucketName, objectKey, annotationName);
    }

    /// <summary>Provides the <c>AnnotationName</c> API.</summary>
    public string AnnotationName { get; private set; } = null!;

    /// <summary>Provides the <c>EnableChecksum</c> API.</summary>
    public bool EnableChecksum { get; set; }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

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

    internal override void Reset()
    {
        AnnotationName = null!;
        VersionId = null;
        RequestPayer = Payer.Unknown;
        EnableChecksum = false;
        BucketName = null!;
        ObjectKey = null!;
        base.Reset();
    }
}