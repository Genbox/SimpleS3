using Genbox.SimpleS3.Core.Network.Responses.Interfaces;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

/// <summary>Provides the <c>GetObjectAclResponse</c> API.</summary>
public class GetObjectAclResponse : BaseResponse, IHasRequestCharged
{
    /// <summary>Provides the <c>GetObjectAclResponse</c> API.</summary>
    public GetObjectAclResponse()
    {
        Grants = new List<S3Grant>();
    }

    /// <summary>Provides the <c>Owner</c> API.</summary>
    public S3Identity? Owner { get; internal set; }

    /// <summary>Provides the <c>Grants</c> API.</summary>
    public IList<S3Grant> Grants { get; }

    /// <summary>Provides the <c>RequestCharged</c> API.</summary>
    public bool RequestCharged { get; internal set; }
}