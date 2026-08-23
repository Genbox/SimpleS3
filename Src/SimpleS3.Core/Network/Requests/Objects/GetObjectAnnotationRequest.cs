using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

public sealed class GetObjectAnnotationRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer
{
    internal GetObjectAnnotationRequest() : base(HttpMethodType.GET) {}
    public GetObjectAnnotationRequest(string bucketName, string objectKey, string annotationName) : this() => Initialize(bucketName, objectKey, annotationName);
    public string BucketName { get; set; } = null!;
    public string ObjectKey { get; set; } = null!;
    public string AnnotationName { get; private set; } = null!;
    public string? VersionId { get; set; }
    public Payer RequestPayer { get; set; }
    public bool EnableChecksum { get; set; }

    internal void Initialize(string bucketName, string objectKey, string annotationName)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
        AnnotationName = annotationName;
    }

    public override void Reset()
    {
        AnnotationName = null!;
        VersionId = null;
        RequestPayer = Payer.Unknown;
        EnableChecksum = false;
        base.Reset();
    }
}