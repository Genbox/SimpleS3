using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Internals.Pools;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

[PublicAPI]
public class S3DeleteInfo : IPooledObject, IHasObjectKey, IHasVersionId
{
    internal S3DeleteInfo() {}

    public S3DeleteInfo(string objectKey, string? versionId = null)
    {
        Initialize(objectKey, versionId);
    }

    public string ObjectKey { get; set; }
    public string? VersionId { get; set; }

    public void Reset()
    {
        //Do nothing as both variables are set through initialize
    }

    public void Initialize(string objectKey, string? versionId = null)
    {
        ObjectKey = objectKey;
        VersionId = versionId;
    }
}