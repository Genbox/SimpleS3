namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Response containing a bucket's ABAC status.</summary>
public sealed class GetBucketAbacResponse : BaseResponse
{
    /// <summary>Whether ABAC is enabled.</summary>
    public bool Enabled { get; internal set; }
}