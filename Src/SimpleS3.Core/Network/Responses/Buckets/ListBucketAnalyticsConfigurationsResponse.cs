using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Response containing bucket analytics configurations.</summary>
public sealed class ListBucketAnalyticsConfigurationsResponse : BaseResponse
{
    /// <summary>Analytics configurations.</summary>
    public IList<S3AnalyticsConfiguration> Configurations { get; } = new List<S3AnalyticsConfiguration>();

    /// <summary>Whether more results are available.</summary>
    public bool IsTruncated { get; internal set; }

    /// <summary>Token supplied in the request.</summary>
    public string? ContinuationToken { get; internal set; }

    /// <summary>Token to request the next page.</summary>
    public string? NextContinuationToken { get; internal set; }
}