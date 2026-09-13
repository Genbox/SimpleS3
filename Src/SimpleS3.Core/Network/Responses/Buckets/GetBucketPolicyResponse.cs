namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>GetBucketPolicyResponse</c> API.</summary>
public class GetBucketPolicyResponse : BaseResponse
{
    /// <summary>Provides the <c>Policy</c> API.</summary>
    public string? Policy { get; internal set; }
}