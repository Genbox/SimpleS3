using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;

namespace Genbox.SimpleS3.Core.Network.Requests.Buckets
{
    /// <summary>Returns the lifecycle configuration information set on the bucket.</summary>
    public class GetBucketLifecycleConfigurationRequest : BaseRequest, IHasBucketName
    {
        internal GetBucketLifecycleConfigurationRequest() : base(HttpMethod.GET) { }

        public GetBucketLifecycleConfigurationRequest(string bucketName) : this()
        {
            Initialize(bucketName);
        }

        public string BucketName { get; set; }

        /// <summary>
        /// The account id of the expected bucket owner. If the bucket is owned by a different account, the request will fail with an HTTP 403 (Access Denied) error.
        /// </summary>
        public string? ExpectedBucketOwner { get; set; }

        internal void Initialize(string bucketName)
        {
            BucketName = bucketName;
        }

        public override void Reset()
        {
            ExpectedBucketOwner = null;
            base.Reset();
        }
    }
}