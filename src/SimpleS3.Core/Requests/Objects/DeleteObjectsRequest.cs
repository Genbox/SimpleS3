using System.Collections.Generic;
using System.Linq;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Requests.Objects.Types;

namespace Genbox.SimpleS3.Core.Requests.Objects
{
    /// <summary>
    /// The Multi-Object Delete operation enables you to delete multiple objects from a bucket using a single HTTP request. If you know the object
    /// keys that you want to delete, then this operation provides a suitable alternative to sending individual delete requests (see DELETE Object), reducing
    /// per-request overhead.
    /// </summary>
    public class DeleteObjectsRequest : BaseRequest
    {
        public DeleteObjectsRequest(string bucketName, IEnumerable<S3DeleteInfo> resources) : base(HttpMethod.POST, bucketName, string.Empty)
        {
            Mfa = new MfaAuthenticationBuilder();
            Objects = resources.ToList();
            Quiet = true;
        }

        /// <summary>If multi-factor approval is activated, you need to supply MFA information.</summary>
        public MfaAuthenticationBuilder Mfa { get; internal set; }

        /// <summary>In quiet mode the response includes only keys where the delete operation encountered an error.</summary>
        public bool Quiet { get; set; }

        public IList<S3DeleteInfo> Objects { get; }
    }
}