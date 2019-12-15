using System;
using Genbox.SimpleS3.Core.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces
{
    [PublicAPI]
    public interface IHasLock
    {
        /// <summary>The Object Lock mode, if any, that should be applied to this object.</summary>
        LockMode LockMode { get; set; }

        /// <summary>The date and time when the Object Lock retention period will expire.</summary>
        DateTimeOffset? LockRetainUntil { get; set; }
    }
}