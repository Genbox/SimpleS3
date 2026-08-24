using Genbox.SimpleS3.Core.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3PartInfo</c> API.</summary>
[PublicAPI]
public class S3PartInfo(string eTag, int partNumber, byte[]? checksum = null, ChecksumAlgorithm checksumAlgorithm = ChecksumAlgorithm.Unknown)
{
    /// <summary>Provides the <c>ETag</c> API.</summary>
    public string ETag { get; } = eTag;
    /// <summary>Provides the <c>PartNumber</c> API.</summary>
    public int PartNumber { get; } = partNumber;
    /// <summary>Provides the <c>Checksum</c> API.</summary>
    public byte[]? Checksum { get; } = checksum;
    /// <summary>Provides the <c>ChecksumAlgorithm</c> API.</summary>
    public ChecksumAlgorithm ChecksumAlgorithm { get; } = checksumAlgorithm;
}