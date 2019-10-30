using Genbox.HttpBuilders;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.Properties
{
    [PublicAPI]
    public interface IHasCacheControl
    {
        /// <summary>Can be used to specify caching behavior along the request/reply chain.</summary>
        CacheControlBuilder CacheControl { get; }
    }
}