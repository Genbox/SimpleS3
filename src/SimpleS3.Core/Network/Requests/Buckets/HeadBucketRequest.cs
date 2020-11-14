using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets
{
    /// <summary>
    /// This operation is useful to determine if a bucket exists and you have permission to access it. The operation returns a 200 OK if the bucket
    /// exists and you have permission to access it. Otherwise, the operation might return responses such as 404 Not Found and 403 Forbidden.
    /// </summary>
    public class HeadBucketRequest : BaseRequest, IHasBucketName
    {
        public HeadBucketRequest(string bucketName) : base(HttpMethod.HEAD)
        {
            BucketName = bucketName;
        }

        public string BucketName { get; set; }
    }
}