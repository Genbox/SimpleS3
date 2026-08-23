using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

public sealed class ListObjectAnnotationsRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer
{
    internal ListObjectAnnotationsRequest() : base(HttpMethodType.GET) {}
    public ListObjectAnnotationsRequest(string bucketName, string objectKey) : this() => Initialize(bucketName, objectKey);
    public string BucketName { get; set; } = null!;
    public string ObjectKey { get; set; } = null!;
    public string? AnnotationPrefix { get; set; }
    public string? ContinuationToken { get; set; }
    public int? MaxAnnotationResults { get; set; }
    public string? VersionId { get; set; }
    public Payer RequestPayer { get; set; }

    internal void Initialize(string bucketName, string objectKey)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
    }

    public override void Reset()
    {
        AnnotationPrefix = null;
        ContinuationToken = null;
        MaxAnnotationResults = null;
        VersionId = null;
        RequestPayer = Payer.Unknown;
        base.Reset();
    }
}