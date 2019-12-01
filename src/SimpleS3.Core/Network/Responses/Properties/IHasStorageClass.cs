using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Responses.Properties
{
    public interface IHasStorageClass
    {
        StorageClass StorageClass { get; }
    }
}