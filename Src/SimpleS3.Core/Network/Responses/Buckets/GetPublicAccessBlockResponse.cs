namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>GetPublicAccessBlockResponse</c> API.</summary>
public sealed class GetPublicAccessBlockResponse : BaseResponse
{
    /// <summary>Provides the <c>BlockPublicAcls</c> API.</summary>
    public bool BlockPublicAcls { get; internal set; }

    /// <summary>Provides the <c>IgnorePublicAcls</c> API.</summary>
    public bool IgnorePublicAcls { get; internal set; }

    /// <summary>Provides the <c>BlockPublicPolicy</c> API.</summary>
    public bool BlockPublicPolicy { get; internal set; }

    /// <summary>Provides the <c>RestrictPublicBuckets</c> API.</summary>
    public bool RestrictPublicBuckets { get; internal set; }
}