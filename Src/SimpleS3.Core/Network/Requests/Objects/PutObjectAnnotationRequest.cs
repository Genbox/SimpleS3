using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>Provides the <c>PutObjectAnnotationRequest</c> API.</summary>
public sealed class PutObjectAnnotationRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer, IHasContent, ISupportStreaming, IHasContentMd5, IHasChecksumProperties
{
    internal PutObjectAnnotationRequest() : base(HttpMethodType.PUT) {}

    /// <summary>Provides the <c>PutObjectAnnotationRequest</c> API.</summary>
    public PutObjectAnnotationRequest(string bucketName, string objectKey, string annotationName, Stream content) : this()
    {
        Initialize(bucketName, objectKey, annotationName, content);
    }

    /// <summary>Provides the <c>AnnotationName</c> API.</summary>
    public string AnnotationName { get; private set; } = null!;

    /// <summary>Provides the <c>ObjectIfMatch</c> API.</summary>
    public string? ObjectIfMatch { get; set; }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Provides the <c>ChecksumAlgorithm</c> API.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; set; }

    /// <summary>Provides the <c>Checksum</c> API.</summary>
    public byte[]? Checksum { get; set; }

    /// <summary>Provides the <c>Content</c> API.</summary>
    public Stream? Content { get; internal set; }

    /// <summary>Provides the <c>ContentMd5</c> API.</summary>
    public byte[]? ContentMd5 { get; set; }

    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; set; } = null!;

    /// <summary>Provides the <c>RequestPayer</c> API.</summary>
    public Payer RequestPayer { get; set; }

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; set; }

    internal void Initialize(string bucketName, string objectKey, string annotationName, Stream content)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
        AnnotationName = annotationName;
        Content = content;
    }

    internal override void Reset()
    {
        AnnotationName = null!;
        Content = null;
        VersionId = null;
        RequestPayer = Payer.Unknown;
        ObjectIfMatch = null;
        ContentMd5 = null;
        ChecksumAlgorithm = ChecksumAlgorithm.Unknown;
        Checksum = null;
        BucketName = null!;
        ObjectKey = null!;
        base.Reset();
    }
}