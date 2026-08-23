namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

public sealed class S3ObjectAttributePart(int partNumber, long size)
{
    public int PartNumber { get; } = partNumber;
    public long Size { get; } = size;
    public byte[]? ChecksumCrc32 { get; internal set; }
    public byte[]? ChecksumCrc32C { get; internal set; }
    public byte[]? ChecksumCrc64Nvme { get; internal set; }
    public byte[]? ChecksumSha1 { get; internal set; }
    public byte[]? ChecksumSha256 { get; internal set; }
}