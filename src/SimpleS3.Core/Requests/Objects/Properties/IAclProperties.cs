using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Requests.Objects.Properties
{
    [PublicAPI]
    public interface IAclProperties
    {
        /// <summary>The canned ACL to apply to the object.</summary>
        ObjectCannedAcl Acl { get; set; }

        /// <summary>
        /// To explicitly grant access permissions to specific AWS accounts or a group, use the following headers. Each maps to specific permissions
        /// that Amazon S3 supports in an ACL.
        /// </summary>
        AclBuilder AclGrantRead { get; }

        /// <summary>Grants permission to read the object ACL.</summary>
        AclBuilder AclGrantReadAcp { get; }

        /// <summary>Grants permission to write the ACL for the applicable object.</summary>
        AclBuilder AclGrantWriteAcp { get; }

        /// <summary>Grants READ, READ_ACP, and WRITE_ACP permissions on the object.</summary>
        AclBuilder AclGrantFullControl { get; }
    }
}