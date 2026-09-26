using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3DeleteMarker</c> API.</summary>
public class S3DeleteMarker : IHasObjectKey, IHasVersionId
{
    internal S3DeleteMarker(bool isLatest, string objectKey, DateTimeOffset lastModified, S3Identity owner, string versionId)
    {
        IsLatest = isLatest;
        LastModified = lastModified;
        Owner = owner;
        ObjectKey = objectKey;
        VersionId = versionId;
    }

    /// <summary>Provides the <c>IsLatest</c> API.</summary>
    public bool IsLatest { get; }

    /// <summary>Provides the <c>LastModified</c> API.</summary>
    public DateTimeOffset LastModified { get; }

    /// <summary>Provides the <c>Owner</c> API.</summary>
    public S3Identity Owner { get; }

    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; internal set; }

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string VersionId { get; }
}