namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>HeadBucketResponse</c> API.</summary>
public class HeadBucketResponse : BaseResponse
{
    /// <summary>The region containing the bucket, including when reported on an unsuccessful request.</summary>
    public string? BucketRegion { get; internal set; }
}