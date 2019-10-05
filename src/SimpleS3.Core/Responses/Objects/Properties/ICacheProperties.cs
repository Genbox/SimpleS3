using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Responses.Objects.Properties
{
    [PublicAPI]
    public interface ICacheProperties
    {
        /// <summary>Can be used to specify caching behavior along the request/reply chain.</summary>
        string CacheControl { get; }

        /// <summary>The entity tag is a hash of the object. The ETag reflects changes only to the contents of an object, not its metadata.</summary>
        string ETag { get; }
    }
}