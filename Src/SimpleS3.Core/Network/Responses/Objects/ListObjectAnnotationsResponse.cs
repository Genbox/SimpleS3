using Genbox.SimpleS3.Core.Network.Responses.Interfaces;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

public sealed class ListObjectAnnotationsResponse : BaseResponse, IHasRequestCharged, IHasVersionId
{
    public IList<S3Annotation> Annotations { get; } = new List<S3Annotation>();
    public string? BucketName { get; internal set; }
    public string? ObjectKey { get; internal set; }
    public string? AnnotationPrefix { get; internal set; }
    public int? MaxAnnotationResults { get; internal set; }
    public int AnnotationCount { get; internal set; }
    public string? ContinuationToken { get; internal set; }
    public string? NextContinuationToken { get; internal set; }
    public bool RequestCharged { get; internal set; }
    public string? VersionId { get; internal set; }
}