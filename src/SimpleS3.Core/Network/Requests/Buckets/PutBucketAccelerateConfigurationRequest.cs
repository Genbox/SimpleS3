using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets
{
    /// <summary>
    /// Sets the accelerate configuration of an existing bucket. Amazon S3 Transfer Acceleration is a bucket-level feature that enables you to
    /// perform faster data transfers to Amazon S3. To use this operation, you must have permission to perform the s3:PutAccelerateConfiguration action. The
    /// bucket owner has this permission by default. The bucket owner can grant this permission to others.
    /// </summary>
    public class PutBucketAccelerateConfigurationRequest : BaseRequest, IHasBucketName
    {
        internal PutBucketAccelerateConfigurationRequest() : base(HttpMethod.PUT)
        {
        }

        public PutBucketAccelerateConfigurationRequest(string bucketName, bool enabled) : this()
        {
            Initialize(bucketName, enabled);
        }

        internal void Initialize(string bucketName, bool enabled)
        {
            BucketName = bucketName;
            AccelerationEnabled = enabled;
        }

        public string BucketName { get; set; }
        public bool AccelerationEnabled { get; set; }
    }
}