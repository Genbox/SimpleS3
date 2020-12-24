using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets
{
    /// <summary>
    /// Deletes the tags from the bucket. o use this operation, you must have permission to perform the s3:PutBucketTagging action. By default, the
    /// bucket owner has this permission and can grant this permission to others.
    /// </summary>
    public class DeleteBucketTaggingRequest : BaseRequest, IHasBucketName
    {
        public DeleteBucketTaggingRequest(string bucketName) : base(HttpMethod.DELETE)
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