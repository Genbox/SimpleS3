using Genbox.SimpleS3.Core.Network.Responses.Interfaces;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects;

/// <summary>Provides the <c>DeleteObjectsResponse</c> API.</summary>
public class DeleteObjectsResponse : BaseResponse, IHasRequestCharged
{
    /// <summary>Provides the <c>Deleted</c> API.</summary>
    public IList<S3DeletedObject> Deleted { get; } = new List<S3DeletedObject>();

    /// <summary>Provides the <c>Errors</c> API.</summary>
    public IList<S3DeleteError> Errors { get; } = new List<S3DeleteError>();

    /// <summary>Provides the <c>RequestCharged</c> API.</summary>
    public bool RequestCharged { get; internal set; }
}