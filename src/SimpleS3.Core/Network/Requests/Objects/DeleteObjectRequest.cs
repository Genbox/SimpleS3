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
    public class DeleteObjectRequest : BaseRequest, IHasVersionId, IHasRequestPayer
    {
        public DeleteObjectRequest(string bucketName, string objectKey) : base(HttpMethod.DELETE, bucketName, objectKey)
        {
            Mfa = new MfaAuthenticationBuilder();
        }

        /// <summary>If multi-factor approval is activated, you need to supply MFA information.</summary>
        public MfaAuthenticationBuilder Mfa { get; internal set; }

        /// <summary>
        /// Specifies whether you want to delete this object even if it has a Governance-type Object Lock in place. You must have sufficient permissions
        /// to perform this operation.
        /// </summary>
        public bool? BypassGovernanceRetention { get; set; }

        public string VersionId { get; set; }
        public Payer RequestPayer { get; set; }
    }
}