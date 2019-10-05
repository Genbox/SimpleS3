using System;
using Genbox.SimpleS3.Core.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Responses.Objects.Properties
{
    [PublicAPI]
    public interface ILockProperties
    {
        /// <summary>
        /// The Object Lock mode, if any, that's in effect for this object. This header is only returned if the requester has the s3:GetObjectRetention
        /// permission.
        /// </summary>
        LockMode LockMode { get; }

        /// <summary>
        /// The date and time when the Object Lock retention period expires. This header is only returned if the requester has the s3:GetObjectRetention
        /// permission.
        /// </summary>
        DateTimeOffset LockRetainUntilDate { get; }

        /// <summary>
        /// Specifies whether a legal hold is in effect for this object. This header is only returned if the requester has the s3:GetObjectLegalHold
        /// permission. This header is not returned if the specified version of this object has never had a legal hold applied.
        /// </summary>
        bool LockLegalHold { get; }
    }
}