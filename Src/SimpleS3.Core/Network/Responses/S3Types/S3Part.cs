using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types;

/// <summary>Provides the <c>S3Part</c> API.</summary>
public class S3Part(int partNumber, DateTimeOffset lastModified, long size, string? eTag) : IHasETag
{
    /// <summary>Provides the <c>PartNumber</c> API.</summary>
    public int PartNumber { get; } = partNumber;
    /// <summary>Provides the <c>LastModified</c> API.</summary>
    public DateTimeOffset LastModified { get; } = lastModified;
    /// <summary>Provides the <c>Size</c> API.</summary>
    public long Size { get; } = size;
    /// <summary>Provides the <c>ETag</c> API.</summary>
    public string? ETag { get; } = eTag;

    /// <summary>Provides the <c>ToString</c> API.</summary>
    public override string ToString() => $"Part: {PartNumber}";
}