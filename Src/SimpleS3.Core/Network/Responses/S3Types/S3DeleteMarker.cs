using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

public class S3DeleteMarker(bool isLatest, string objectKey, DateTimeOffset lastModified, S3Identity owner, string versionId) : IHasObjectKey, IHasVersionId
{
    public bool IsLatest { get; } = isLatest;
    public DateTimeOffset LastModified { get; } = lastModified;
    public S3Identity Owner { get; } = owner;
    public string ObjectKey { get; internal set; } = objectKey;
    public string VersionId { get; } = versionId;
}