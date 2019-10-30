using Genbox.SimpleS3.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets
{
    /// <summary>
    /// Deletes a bucket. All objects (including all object versions and delete markers) in the bucket must be deleted before the bucket itself can
    /// be deleted.
    /// </summary>
    public class DeleteBucketRequest : BaseRequest
    {
        public DeleteBucketRequest(string bucketName) : base(HttpMethod.DELETE, bucketName, string.Empty)
        {
        }
    }
}