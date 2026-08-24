namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>GetBucketLocationResponse</c> API.</summary>
public sealed class GetBucketLocationResponse : BaseResponse
{
    /// <summary>Provides the <c>Location</c> API.</summary>
    public string? Location { get; internal set; }
}