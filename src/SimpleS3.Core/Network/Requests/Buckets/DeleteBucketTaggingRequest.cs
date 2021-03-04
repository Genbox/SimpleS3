using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets
{
    /// <summary>
    /// Deletes the tags from the bucket. o use this operation, you must have permission to perform the s3:PutBucketTagging action. By default, the
    /// bucket owner has this permission and can grant this permission to others.
    /// </summary>
    public class DeleteBucketTaggingRequest : BaseRequest, IHasBucketName
    {
        internal DeleteBucketTaggingRequest() : base(HttpMethod.DELETE) { }

        public DeleteBucketTaggingRequest(string bucketName) : this()
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