using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

/// <summary>Provides the <c>IHasCache</c> API.</summary>
[PublicAPI]
public interface IHasCache : IHasETag
{
    /// <summary>Can be used to specify caching behavior along the request/reply chain.</summary>
    string? CacheControl { get; }
}