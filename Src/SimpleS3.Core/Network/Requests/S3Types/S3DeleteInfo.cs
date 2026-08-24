using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Internals.Pools;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3DeleteInfo</c> API.</summary>
[PublicAPI]
public class S3DeleteInfo : IPooledObject, IHasObjectKey, IHasVersionId
{
    internal S3DeleteInfo() {}

    /// <summary>Provides the <c>S3DeleteInfo</c> API.</summary>
    public S3DeleteInfo(string objectKey, string? versionId = null)
    {
        Initialize(objectKey, versionId);
    }

    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; set; } = null!;
    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; set; }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public void Reset()
    {
        //Do nothing as both variables are set through initialize
    }

    /// <summary>Provides the <c>Initialize</c> API.</summary>
    public void Initialize(string objectKey, string? versionId = null)
    {
        ObjectKey = objectKey;
        VersionId = versionId;
    }
}