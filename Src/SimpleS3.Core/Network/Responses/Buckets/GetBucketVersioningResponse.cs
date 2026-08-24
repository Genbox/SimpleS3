namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>GetBucketVersioningResponse</c> API.</summary>
public class GetBucketVersioningResponse : BaseResponse
{
    /// <summary>Provides the <c>Status</c> API.</summary>
    public bool Status { get; internal set; }
    /// <summary>Provides the <c>MfaDelete</c> API.</summary>
    public bool MfaDelete { get; internal set; }
}