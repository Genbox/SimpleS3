namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3ObjectAttributePart</c> API.</summary>
public sealed class S3ObjectAttributePart
{
    internal S3ObjectAttributePart(int partNumber, long size)
    {
        PartNumber = partNumber;
        Size = size;
    }

    /// <summary>Provides the <c>PartNumber</c> API.</summary>
    public int PartNumber { get; }

    /// <summary>Provides the <c>Size</c> API.</summary>
    public long Size { get; }

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

    /// <summary>The Base64-decoded SHA512 checksum, when returned by the provider.</summary>
    public byte[]? ChecksumSha512 { get; internal set; }

    /// <summary>The Base64-decoded MD5 checksum, when returned by the provider.</summary>
    public byte[]? ChecksumMd5 { get; internal set; }

    /// <summary>The Base64-decoded XXHASH64 checksum, when returned by the provider.</summary>
    public byte[]? ChecksumXxHash64 { get; internal set; }

    /// <summary>The Base64-decoded XXHASH3 checksum, when returned by the provider.</summary>
    public byte[]? ChecksumXxHash3 { get; internal set; }

    /// <summary>The Base64-decoded XXHASH128 checksum, when returned by the provider.</summary>
    public byte[]? ChecksumXxHash128 { get; internal set; }
}