using System;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Properties;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets
{
    public class GetBucketLockConfigurationResponse : BaseResponse, IHasLock
    {
        public LockMode LockMode { get; internal set; }
        public DateTimeOffset? LockRetainUntil { get; internal set; }
    }
}