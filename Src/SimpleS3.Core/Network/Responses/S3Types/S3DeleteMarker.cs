using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3DeleteMarker</c> API.</summary>
public class S3DeleteMarker(bool isLatest, string objectKey, DateTimeOffset lastModified, S3Identity owner, string versionId) : IHasObjectKey, IHasVersionId
{
    /// <summary>Provides the <c>IsLatest</c> API.</summary>
    public bool IsLatest { get; } = isLatest;

    /// <summary>Provides the <c>LastModified</c> API.</summary>
    public DateTimeOffset LastModified { get; } = lastModified;

    /// <summary>Provides the <c>Owner</c> API.</summary>
    public S3Identity Owner { get; } = owner;

    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; internal set; } = objectKey;

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string VersionId { get; } = versionId;
}