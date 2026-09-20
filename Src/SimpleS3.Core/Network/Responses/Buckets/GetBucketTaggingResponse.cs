namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>GetBucketTaggingResponse</c> API.</summary>
public class GetBucketTaggingResponse : BaseResponse
{
    /// <summary>Provides the <c>Tags</c> API.</summary>
    public IDictionary<string, string> Tags { get; } = new Dictionary<string, string>(StringComparer.Ordinal);
}