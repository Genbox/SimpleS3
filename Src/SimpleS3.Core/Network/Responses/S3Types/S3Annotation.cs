using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

public sealed class S3Annotation(string name, DateTimeOffset lastModified, long size)
{
    public string Name { get; } = name;
    public DateTimeOffset LastModified { get; } = lastModified;
    public long Size { get; } = size;
    public string? ETag { get; internal set; }
    public IList<ChecksumAlgorithm> ChecksumAlgorithms { get; } = new List<ChecksumAlgorithm>();
    public ReplicationStatus ReplicationStatus { get; internal set; }
}