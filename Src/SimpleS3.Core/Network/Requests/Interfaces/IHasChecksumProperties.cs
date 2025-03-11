using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces;

public interface IHasChecksumProperties
{
    ChecksumAlgorithm ChecksumAlgorithm { get; }
    byte[]? Checksum { get; }
}