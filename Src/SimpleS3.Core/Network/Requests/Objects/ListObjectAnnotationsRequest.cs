using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>Provides the <c>ListObjectAnnotationsRequest</c> API.</summary>
public sealed class ListObjectAnnotationsRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer
{
    internal ListObjectAnnotationsRequest() : base(HttpMethodType.GET) {}

    /// <summary>Provides the <c>ListObjectAnnotationsRequest</c> API.</summary>
    public ListObjectAnnotationsRequest(string bucketName, string objectKey) : this()
    {
        Initialize(bucketName, objectKey);
    }

    /// <summary>Provides the <c>AnnotationPrefix</c> API.</summary>
    public string? AnnotationPrefix { get; set; }

    /// <summary>Provides the <c>ContinuationToken</c> API.</summary>
    public string? ContinuationToken { get; set; }

    /// <summary>Provides the <c>MaxAnnotationResults</c> API.</summary>
    public int? MaxAnnotationResults { get; set; }

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

    internal override void Reset()
    {
        AnnotationPrefix = null;
        ContinuationToken = null;
        MaxAnnotationResults = null;
        VersionId = null;
        RequestPayer = Payer.Unknown;
        BucketName = null!;
        ObjectKey = null!;
        base.Reset();
    }
}