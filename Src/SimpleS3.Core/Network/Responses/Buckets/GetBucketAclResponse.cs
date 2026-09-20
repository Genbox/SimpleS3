using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>GetBucketAclResponse</c> API.</summary>
public sealed class GetBucketAclResponse : BaseResponse
{
    /// <summary>Provides the <c>GetBucketAclResponse</c> API.</summary>
    public GetBucketAclResponse()
    {
        Grants = new List<S3Grant>();
    }

    /// <summary>Provides the <c>Owner</c> API.</summary>
    public S3Identity? Owner { get; internal set; }

    /// <summary>Provides the <c>Grants</c> API.</summary>
    public IList<S3Grant> Grants { get; }
}