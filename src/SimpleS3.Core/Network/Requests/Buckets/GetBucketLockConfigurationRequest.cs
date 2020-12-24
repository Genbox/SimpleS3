﻿using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets
{
    /// <summary>
    /// Gets the Object Lock configuration for a bucket. The rule specified in the Object Lock configuration will be applied by default to every new
    /// object placed in the specified bucket.
    /// </summary>
    public class GetBucketLockConfigurationRequest : BaseRequest, IHasBucketName
    {
        public GetBucketLockConfigurationRequest(string bucketName) : base(HttpMethod.GET)
        {
            Initialize(bucketName);
        }

        internal void Initialize(string bucketName)
        {
            BucketName = bucketName;
        }

        public string BucketName { get; set; }
    }
}