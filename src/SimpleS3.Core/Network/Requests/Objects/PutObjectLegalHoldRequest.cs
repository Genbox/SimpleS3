using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Properties;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects
{
    /// <summary>Sets an object's current Legal Hold status.</summary>
    public class PutObjectLegalHoldRequest : BaseRequest, IHasBucketName, IHasObjectKey, IHasVersionId, IHasRequestPayer, IHasContentMd5
    {
        public PutObjectLegalHoldRequest(string bucketName, string objectKey, bool legalHold) : base(HttpMethod.PUT)
        {
            BucketName = bucketName;
            ObjectKey = objectKey;
            LegalHold = legalHold;
            ForceContentMd5 = () => true;
        }

        public string BucketName { get; set; }
        public string ObjectKey { get; set; }
        public Payer RequestPayer { get; set; }
        public string VersionId { get; set; }
        public byte[] ContentMd5 { get; set; }
        public bool LegalHold { get; set; }
    }
}