using System;
using Genbox.HttpBuilders;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces
{
    [PublicAPI]
    public interface IHasCache
    {
        /// <summary>Return the object only if it has been modified since the specified time, otherwise return a 304 (not modified).</summary>
        DateTimeOffset? IfModifiedSince { get; set; }

        /// <summary>Return the object only if it has not been modified since the specified time, otherwise return a 412 (precondition failed).</summary>
        DateTimeOffset? IfUnmodifiedSince { get; set; }

        /// <summary>Return the object only if its entity tag (ETag) is the same as the one specified; otherwise, return a 412 (precondition failed).</summary>
        ETagBuilder IfETagMatch { get; }

        /// <summary>Return the object only if its entity tag (ETag) is different from the one specified; otherwise, return a 304 (not modified).</summary>
        ETagBuilder IfETagNotMatch { get; }
    }
}