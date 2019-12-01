using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Properties;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects
{
    /// <summary>
    /// The DELETE operation removes the null version (if there is one) of an object and inserts a delete marker, which becomes the current version
    /// of the object. If there isn't a null version, Amazon S3 does not remove any objects.
    /// </summary>
    public class DeleteObjectRequest : BaseRequest, IHasVersionId, IHasRequestPayer, IHasBypassGovernanceRetention, IHasObjectKey, IHasBucketName, IHasMfa
    {
        public DeleteObjectRequest(string bucketName, string objectKey) : base(HttpMethod.DELETE)
        {
            BucketName = bucketName;
            ObjectKey = objectKey;
            Mfa = new MfaAuthenticationBuilder();
        }

        public string BucketName { get; set; }
        public bool? BypassGovernanceRetention { get; set; }
        public MfaAuthenticationBuilder Mfa { get; internal set; }
        public string ObjectKey { get; set; }
        public Payer RequestPayer { get; set; }
        public string VersionId { get; set; }
    }
}