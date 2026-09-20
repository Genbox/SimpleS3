namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3ObjectAttributePart</c> API.</summary>
public sealed class S3ObjectAttributePart(int partNumber, long size)
{
    /// <summary>Provides the <c>PartNumber</c> API.</summary>
    public int PartNumber { get; } = partNumber;

    /// <summary>Provides the <c>Size</c> API.</summary>
    public long Size { get; } = size;

    /// <summary>Provides the <c>ChecksumCrc32</c> API.</summary>
    public byte[]? ChecksumCrc32 { get; internal set; }

    /// <summary>Provides the <c>ChecksumCrc32C</c> API.</summary>
    public byte[]? ChecksumCrc32C { get; internal set; }

    /// <summary>Provides the <c>ChecksumCrc64Nvme</c> API.</summary>
    public byte[]? ChecksumCrc64Nvme { get; internal set; }

    /// <summary>Provides the <c>ChecksumSha1</c> API.</summary>
    public byte[]? ChecksumSha1 { get; internal set; }

    /// <summary>Provides the <c>ChecksumSha256</c> API.</summary>
    public byte[]? ChecksumSha256 { get; internal set; }
}