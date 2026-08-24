using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces;

/// <summary>Provides the <c>IHasChecksumProperties</c> API.</summary>
public interface IHasChecksumProperties
{
    /// <summary>Provides the <c>ChecksumAlgorithm</c> API.</summary>
    ChecksumAlgorithm ChecksumAlgorithm { get; }
    /// <summary>Provides the <c>Checksum</c> API.</summary>
    byte[]? Checksum { get; }
}