using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets
{
    /// <summary>
    /// Returns the tag set associated with the bucket. To use this operation, you must have permission to perform the s3:GetBucketTagging action.
    /// By default, the bucket owner has this permission and can grant this permission to others.
    /// </summary>
    public class GetBucketTaggingRequest : BaseRequest, IHasBucketName
    {
        public GetBucketTaggingRequest(string bucketName) : base(HttpMethod.GET)
        {
            BucketName = bucketName;
        }

        public string BucketName { get; set; }
    }
}