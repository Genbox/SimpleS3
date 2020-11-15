using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets
{
    /// <summary>
    /// Create a bucket. By creating the bucket, you become the bucket owner. By default, the bucket is created in the US East (N. Virginia) region.
    /// You can optionally specify a region in the request body. You might choose a region to optimize latency, minimize costs, or address regulatory
    /// requirements.
    /// </summary>
    public class CreateBucketRequest : BaseRequest, IHasBucketAcl, IHasBucketName
    {
        public CreateBucketRequest(string bucketName) : base(HttpMethod.PUT)
        {
            BucketName = bucketName;
            AclGrantRead = new AclBuilder();
            AclGrantWrite = new AclBuilder();
            AclGrantReadAcp = new AclBuilder();
            AclGrantWriteAcp = new AclBuilder();
            AclGrantFullControl = new AclBuilder();
        }

        /// <summary>Enable object locking on the bucket.</summary>
        public bool? EnableObjectLocking { get; set; }

        public BucketCannedAcl Acl { get; set; }
        public AclBuilder AclGrantRead { get; }
        public AclBuilder AclGrantWrite { get; }
        public AclBuilder AclGrantReadAcp { get; }
        public AclBuilder AclGrantWriteAcp { get; }
        public AclBuilder AclGrantFullControl { get; }
        public string BucketName { get; set; }
    }
}