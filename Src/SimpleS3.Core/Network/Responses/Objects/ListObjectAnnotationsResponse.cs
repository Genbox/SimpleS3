using Genbox.SimpleS3.Core.Network.Responses.Interfaces;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

/// <summary>Provides the <c>ListObjectAnnotationsResponse</c> API.</summary>
public sealed class ListObjectAnnotationsResponse : BaseResponse, IHasRequestCharged, IHasVersionId
{
    /// <summary>Provides the <c>Annotations</c> API.</summary>
    public IList<S3Annotation> Annotations { get; } = new List<S3Annotation>();

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string? BucketName { get; internal set; }

    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string? ObjectKey { get; internal set; }

    /// <summary>Provides the <c>AnnotationPrefix</c> API.</summary>
    public string? AnnotationPrefix { get; internal set; }

    /// <summary>Provides the <c>MaxAnnotationResults</c> API.</summary>
    public int? MaxAnnotationResults { get; internal set; }

    /// <summary>Provides the <c>AnnotationCount</c> API.</summary>
    public int AnnotationCount { get; internal set; }

    /// <summary>Provides the <c>ContinuationToken</c> API.</summary>
    public string? ContinuationToken { get; internal set; }

    /// <summary>Provides the <c>NextContinuationToken</c> API.</summary>
    public string? NextContinuationToken { get; internal set; }

    /// <summary>Provides the <c>RequestCharged</c> API.</summary>
    public bool RequestCharged { get; internal set; }

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; internal set; }
}