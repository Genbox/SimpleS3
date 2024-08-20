using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

public class S3Part(int partNumber, DateTimeOffset lastModified, long size, string? eTag) : IHasETag
{
    public int PartNumber { get; } = partNumber;
    public DateTimeOffset LastModified { get; } = lastModified;
    public long Size { get; } = size;
    public string? ETag { get; } = eTag;

    public override string ToString() => $"Part: {PartNumber}";
}