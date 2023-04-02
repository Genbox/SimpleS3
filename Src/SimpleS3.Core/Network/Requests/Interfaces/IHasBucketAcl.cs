using Genbox.SimpleS3.Core.Builders;
using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces;

public interface IHasBucketAcl
{
    /// <summary>The canned ACL to apply to the bucket you are creating.</summary>
    BucketCannedAcl Acl { get; set; }

    /// <summary>Allows grantee to list the objects in the bucket.</summary>
    AclBuilder AclGrantRead { get; }

    /// <summary>Allows grantee to create, overwrite, and delete any object in the bucket.</summary>
    AclBuilder AclGrantWrite { get; }

    /// <summary>Allows grantee to read the bucket ACL</summary>
    AclBuilder AclGrantReadAcp { get; }

    /// <summary>Allows grantee to write the ACL for the applicable bucket.</summary>
    AclBuilder AclGrantWriteAcp { get; }

    /// <summary>Allows grantee the READ, WRITE, READ_ACP, and WRITE_ACP permissions on the bucket.</summary>
    AclBuilder AclGrantFullControl { get; }
}