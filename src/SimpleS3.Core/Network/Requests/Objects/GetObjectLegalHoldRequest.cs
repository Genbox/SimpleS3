using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Properties;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects
{
    /// <summary>Gets an object's current Legal Hold status.</summary>
    public class GetObjectLegalHoldRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer
    {
        public GetObjectLegalHoldRequest(string bucketName, string objectKey) : base(HttpMethod.GET)
        {
            BucketName = bucketName;
            ObjectKey = objectKey;
        }

        public string BucketName { get; set; }
        public string ObjectKey { get; set; }
        public Payer RequestPayer { get; set; }
        public string VersionId { get; set; }
    }
}