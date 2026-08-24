using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

/// <summary>Provides the <c>IHasStorageClass</c> API.</summary>
public interface IHasStorageClass
{
    /// <summary>Provides the <c>StorageClass</c> API.</summary>
    StorageClass StorageClass { get; }
}