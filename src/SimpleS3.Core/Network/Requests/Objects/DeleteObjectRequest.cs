using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Properties;
using Genbox.SimpleS3.Core.Network.SharedProperties;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects
{
    /// <summary>
    /// The DELETE operation removes the null version (if there is one) of an object and inserts a delete marker, which becomes the current version
    /// of the object. If there isn't a null version, Amazon S3 does not remove any objects.
    /// </summary>
    public class DeleteObjectRequest : BaseRequest, IHasVersionId, IHasRequestPayer, IHasBypassGovernanceRetention, IHasObjectKey, IHasBucketName
    {
        public DeleteObjectRequest(string bucketName, string objectKey) : base(HttpMethod.DELETE)
        {
            BucketName = bucketName;
            ObjectKey = objectKey;
            Mfa = new MfaAuthenticationBuilder();
        }

        /// <summary>If multi-factor approval is activated, you need to supply MFA information.</summary>
        public MfaAuthenticationBuilder Mfa { get; internal set; }

        public bool? BypassGovernanceRetention { get; set; }
        public Payer RequestPayer { get; set; }
        public string VersionId { get; set; }
        public string ObjectKey { get; set; }
        public string BucketName { get; set; }
    }
}