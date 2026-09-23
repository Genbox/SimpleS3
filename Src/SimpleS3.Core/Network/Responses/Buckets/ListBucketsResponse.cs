using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>ListBucketsResponse</c> API.</summary>
public class ListBucketsResponse : BaseResponse
{
    /// <summary>Provides the <c>ListBucketsResponse</c> API.</summary>
    public ListBucketsResponse()
    {
        Buckets = new List<S3Bucket>();
    }

    /// <summary>The owner of the buckets inside <see cref="Buckets" />.</summary>
    public S3Identity? Owner { get; internal set; }

    /// <summary>The list of buckets</summary>
    public IList<S3Bucket> Buckets { get; }

    /// <summary>Opaque token used to retrieve the next page, or <see langword="null" /> when this is the final page.</summary>
    public string? ContinuationToken { get; internal set; }

    /// <summary>The prefix included in the request.</summary>
    public string? Prefix { get; internal set; }
}