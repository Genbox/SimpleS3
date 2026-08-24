namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>GetBucketPolicyStatusResponse</c> API.</summary>
public sealed class GetBucketPolicyStatusResponse : BaseResponse
{
    /// <summary>Provides the <c>IsPublic</c> API.</summary>
    public bool IsPublic { get; internal set; }
}