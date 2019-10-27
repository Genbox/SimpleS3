using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Builders;

namespace Genbox.SimpleS3.Core.Requests.Objects
{
    /// <summary>
    /// The DELETE operation removes the null version (if there is one) of an object and inserts a delete marker, which becomes the current version
    /// of the object. If there isn't a null version, Amazon S3 does not remove any objects.
    /// </summary>
    public class DeleteObjectRequest : BaseRequest
    {
        public DeleteObjectRequest(string bucketName, string objectKey) : base(HttpMethod.DELETE, bucketName, objectKey)
        {
            Mfa = new MfaAuthenticationBuilder();
        }

        public string VersionId { get; set; }

        /// <summary>If multi-factor approval is activated, you need to supply MFA information.</summary>
        public MfaAuthenticationBuilder Mfa { get; internal set; }
    }
}