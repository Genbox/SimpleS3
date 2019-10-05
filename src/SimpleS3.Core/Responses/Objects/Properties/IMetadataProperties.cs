using System.Collections.Generic;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Responses.Objects.Properties
{
    [PublicAPI]
    public interface IMetadataProperties
    {
        /// <summary>
        /// Header starting with this prefix are user-defined metadata. Each one is stored and returned as a set of key-value pairs. Amazon S3 doesn't
        /// validate or interpret user-defined metadata.
        /// </summary>
        IDictionary<string, string> Metadata { get; }
    }
}