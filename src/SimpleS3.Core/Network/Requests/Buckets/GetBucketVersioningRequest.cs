using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets
{
    public class GetBucketVersioningRequest : BaseRequest, IHasBucketName
    {
        internal GetBucketVersioningRequest() : base(HttpMethod.GET) { }

        public GetBucketVersioningRequest(string bucketName) : this()
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