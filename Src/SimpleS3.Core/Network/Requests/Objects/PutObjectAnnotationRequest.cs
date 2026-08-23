using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

public sealed class PutObjectAnnotationRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer, IHasContent, ISupportStreaming, IHasContentMd5, IHasChecksumProperties
{
    internal PutObjectAnnotationRequest() : base(HttpMethodType.PUT) {}
    public PutObjectAnnotationRequest(string bucketName, string objectKey, string annotationName, Stream content) : this() => Initialize(bucketName, objectKey, annotationName, content);
    public string BucketName { get; set; } = null!;
    public string ObjectKey { get; set; } = null!;
    public string AnnotationName { get; private set; } = null!;
    public Stream? Content { get; internal set; }
    public string? VersionId { get; set; }
    public Payer RequestPayer { get; set; }
    public string? ObjectIfMatch { get; set; }
    public byte[]? ContentMd5 { get; set; }
    public ChecksumAlgorithm ChecksumAlgorithm { get; set; }
    public byte[]? Checksum { get; set; }

    internal void Initialize(string bucketName, string objectKey, string annotationName, Stream content)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
        AnnotationName = annotationName;
        Content = content;
    }

    public override void Reset()
    {
        AnnotationName = null!;
        Content = null;
        VersionId = null;
        RequestPayer = Payer.Unknown;
        ObjectIfMatch = null;
        ContentMd5 = null;
        ChecksumAlgorithm = ChecksumAlgorithm.Unknown;
        Checksum = null;
        base.Reset();
    }
}