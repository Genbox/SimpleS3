using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

public class S3DeleteMarker : IHasObjectKey, IHasVersionId
{
    public S3DeleteMarker(bool isLatest, string objectKey, DateTimeOffset lastModified, S3Identity owner, string versionId)
    {
        IsLatest = isLatest;
        ObjectKey = objectKey;
        LastModified = lastModified;
        Owner = owner;
        VersionId = versionId;
    }

    public bool IsLatest { get; }
    public string ObjectKey { get; internal set; }
    public DateTimeOffset LastModified { get; }
    public S3Identity Owner { get; }
    public string VersionId { get; }
}