using System;
using Genbox.HttpBuilders;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.Properties
{
    [PublicAPI]
    public interface IHasResponseHeader
    {
        /// <summary>Sets the Content-Type header of the response.</summary>
        ContentTypeBuilder ResponseContentType { get; }

        /// <summary>Sets the Content-Language header of the response.</summary>
        ContentLanguageBuilder ResponseContentLanguage { get; }

        /// <summary>Sets the Expires header of the response.</summary>
        DateTimeOffset? ResponseExpires { get; set; }

        /// <summary>Sets the Cache-Control header of the response.</summary>
        CacheControlBuilder ResponseCacheControl { get; }

        /// <summary>Sets the Content-Disposition header of the response.</summary>
        ContentDispositionBuilder ResponseContentDisposition { get; }

        /// <summary>Sets the Content-Encoding header of the response.</summary>
        ContentEncodingBuilder ResponseContentEncoding { get; }
    }
}