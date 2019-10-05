using Genbox.SimpleS3.Core.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Responses.Objects.Properties
{
    [PublicAPI]
    public interface IStorageClassProperties
    {
        /// <summary>Provides storage class information of the object. Amazon S3 returns this header for all objects except for Standard storage class objects.</summary>
        StorageClass StorageClass { get; }
    }
}