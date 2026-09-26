using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3DeletedObject</c> API.</summary>
public class S3DeletedObject : IHasDeleteMarker, IHasVersionId, IHasObjectKey
{
    internal S3DeletedObject(string? objectKey, string? versionId, bool isDeleteMarker, string? deleteMarkerVersionId)
    {
        DeleteMarkerVersionId = deleteMarkerVersionId;
        IsDeleteMarker = isDeleteMarker;
        ObjectKey = objectKey;
        VersionId = versionId;
    }

    /// <summary>
    /// The version ID of the delete marker created as a result of the DELETE operation. If you delete a specific object version, the value returned by this header is the version
    /// ID of the object version deleted.
    /// </summary>
    public string? DeleteMarkerVersionId { get; }

    /// <summary>Provides the <c>IsDeleteMarker</c> API.</summary>
    public bool IsDeleteMarker { get; }

    /// <summary>The name of the deleted object.</summary>
    public string? ObjectKey { get; }

    /// <summary>The version ID of the deleted object.</summary>
    public string? VersionId { get; }

    /// <summary>Provides the <c>ToString</c> API.</summary>
    public override string ToString() => ObjectKey ?? string.Empty;
}