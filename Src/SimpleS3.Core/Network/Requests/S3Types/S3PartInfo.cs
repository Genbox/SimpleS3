using Genbox.SimpleS3.Core.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

[PublicAPI]
public class S3PartInfo(string eTag, int partNumber, byte[]? checksum = null, ChecksumAlgorithm checksumAlgorithm = ChecksumAlgorithm.Unknown)
{
    public string ETag { get; } = eTag;
    public int PartNumber { get; } = partNumber;
    public byte[]? Checksum { get; } = checksum;
    public ChecksumAlgorithm ChecksumAlgorithm { get; } = checksumAlgorithm;
}