using Genbox.SimpleS3.Core.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.SharedProperties
{
    [PublicAPI]
    public interface IHasStorageClass
    {
        /// <summary>Provides storage class information of the object. Amazon S3 returns this header for all objects except for Standard storage class objects.</summary>
        StorageClass StorageClass { get; }
    }
}