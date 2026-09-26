using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets;

/// <summary>This implementation of the GET operation returns a list of all buckets owned by the authenticated sender of the request.</summary>
public class ListBucketsRequest() : BaseRequest(HttpMethodType.GET)
{
    /// <summary>Maximum number of buckets to return, from 1 through 10,000.</summary>
    public int? MaxBuckets { get; set; }

    /// <summary>Opaque token used to continue a paginated listing.</summary>
    public string? ContinuationToken { get; set; }

    /// <summary>Limits results to bucket names beginning with this prefix.</summary>
    public string? Prefix { get; set; }

    /// <summary>Limits results to buckets in this region.</summary>
    public string? BucketRegion { get; set; }

    internal override void Reset()
    {
        MaxBuckets = null;
        ContinuationToken = null;
        Prefix = null;
        BucketRegion = null;
        base.Reset();
    }
}