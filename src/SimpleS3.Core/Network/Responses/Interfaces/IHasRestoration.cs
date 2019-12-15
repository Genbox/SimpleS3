using System;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces
{
    [PublicAPI]
    public interface IHasRestoration
    {
        /// <summary>If the object was restored from Amazon Glacier, this value indicates when the object will expire.</summary>
        DateTimeOffset? RestoreExpiresOn { get; }

        /// <summary>If true, indicates that the object is currently being restored from Amazon Glacier.</summary>
        bool RestoreInProgress { get; }
    }
}