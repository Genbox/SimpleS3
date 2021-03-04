using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets
{
    /// <summary>
    /// Deletes a bucket. All objects (including all object versions and delete markers) in the bucket must be deleted before the bucket itself can
    /// be deleted.
    /// </summary>
    public class DeleteBucketRequest : BaseRequest, IHasBucketName
    {
        internal DeleteBucketRequest() : base(HttpMethod.DELETE) { }

        public DeleteBucketRequest(string bucketName) : this()
        {
            Initialize(bucketName);
        }

        public string BucketName { get; set; }

        internal void Initialize(string bucketName)
        {
            BucketName = bucketName;
        }
    }
}