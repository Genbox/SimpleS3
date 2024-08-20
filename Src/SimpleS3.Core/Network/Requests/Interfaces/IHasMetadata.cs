using Genbox.SimpleS3.Core.Builders;

namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces;

public interface IHasMetadata
{
    /// <summary>
    /// Header starting with this prefix are user-defined metadata. Each one is stored and returned as a set of key-value pairs. Amazon S3 doesn't validate or interpret
    /// user-defined metadata.
    /// </summary>
    MetadataBuilder Metadata { get; }
}