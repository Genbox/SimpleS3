using Genbox.HttpBuilders;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Requests.Objects.Properties
{
    [PublicAPI]
    public interface ICacheControl
    {
        /// <summary>Can be used to specify caching behavior along the request/reply chain.</summary>
        CacheControlBuilder CacheControl { get; set; }
    }
}