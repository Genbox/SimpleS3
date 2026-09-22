using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3Part</c> API.</summary>
public class S3Part : IHasETag
{
    internal S3Part(int partNumber, DateTimeOffset lastModified, long size, string? eTag)
    {
        PartNumber = partNumber;
        LastModified = lastModified;
        Size = size;
        ETag = eTag;
    }

    /// <summary>Provides the <c>PartNumber</c> API.</summary>
    public int PartNumber { get; }

    /// <summary>Provides the <c>LastModified</c> API.</summary>
    public DateTimeOffset LastModified { get; }

    /// <summary>Provides the <c>Size</c> API.</summary>
    public long Size { get; }

    /// <summary>The decoded CRC32 checksum, when returned by the provider.</summary>
    public byte[]? ChecksumCrc32 { get; internal set; }

    /// <summary>The decoded CRC32C checksum, when returned by the provider.</summary>
    public byte[]? ChecksumCrc32C { get; internal set; }

    /// <summary>The decoded CRC64NVME checksum, when returned by the provider.</summary>
    public byte[]? ChecksumCrc64Nvme { get; internal set; }

    /// <summary>The decoded SHA1 checksum, when returned by the provider.</summary>
    public byte[]? ChecksumSha1 { get; internal set; }

    /// <summary>The decoded SHA256 checksum, when returned by the provider.</summary>
    public byte[]? ChecksumSha256 { get; internal set; }

    /// <summary>The decoded SHA512 checksum, when returned by the provider.</summary>
    public byte[]? ChecksumSha512 { get; internal set; }

    /// <summary>The decoded MD5 checksum, when returned by the provider.</summary>
    public byte[]? ChecksumMd5 { get; internal set; }

    /// <summary>The decoded XXHASH64 checksum, when returned by the provider.</summary>
    public byte[]? ChecksumXxHash64 { get; internal set; }

    /// <summary>The decoded XXHASH3 checksum, when returned by the provider.</summary>
    public byte[]? ChecksumXxHash3 { get; internal set; }

    /// <summary>The decoded XXHASH128 checksum, when returned by the provider.</summary>
    public byte[]? ChecksumXxHash128 { get; internal set; }

    /// <summary>Provides the <c>ETag</c> API.</summary>
    public string? ETag { get; }

    /// <summary>Provides the <c>ToString</c> API.</summary>
    public override string ToString() => $"Part: {PartNumber}";
}