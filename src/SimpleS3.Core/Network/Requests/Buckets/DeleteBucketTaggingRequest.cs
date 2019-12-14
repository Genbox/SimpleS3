using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Properties;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets
{
    /// <summary>
    /// Deletes the tags from the bucket. o use this operation, you must have permission to perform the s3:PutBucketTagging action. By default, the bucket owner has this permission and can grant this permission to others.
    /// </summary>
    public class DeleteBucketTaggingRequest : BaseRequest, IHasBucketName
    {
        public DeleteBucketTaggingRequest(string bucketName) : base(HttpMethod.DELETE)
        {
            BucketName = bucketName;
        }

        public string BucketName { get; set; }
    }
}