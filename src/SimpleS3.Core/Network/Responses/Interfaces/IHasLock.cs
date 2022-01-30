using Genbox.SimpleS3.Core.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

[PublicAPI]
public interface IHasLock
{
    /// <summary>The Object Lock mode, if any, that's in effect for this object. This header is only returned if the requester
    /// has the s3:GetObjectRetention permission.</summary>
    LockMode LockMode { get; }

    /// <summary>The date and time when the Object Lock retention period expires. This header is only returned if the requester
    /// has the s3:GetObjectRetention permission.</summary>
    DateTimeOffset? LockRetainUntil { get; }
}