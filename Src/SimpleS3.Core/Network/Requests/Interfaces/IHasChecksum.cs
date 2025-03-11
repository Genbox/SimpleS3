using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces;

public interface IHasChecksum
{
    public ChecksumAlgorithm ChecksumAlgorithm { get; set; }
    public ChecksumType ChecksumType { get; set; }
}