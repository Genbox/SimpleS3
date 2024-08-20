namespace Genbox.SimpleS3.Core.Enums;

public enum ObjectOwnership
{
    Unknown = 0,

    /// <summary>Objects uploaded to the bucket change ownership to the bucket owner if the objects are uploaded with the bucket-owner-full-control canned ACL.</summary>
    BucketOwnerPreferred,

    /// <summary>The uploading account will own the object if the object is uploaded with the bucket-owner-full-control canned ACL.</summary>
    ObjectWriter,

    /// <summary>Access control lists (ACLs) are disabled and no longer affect permissions. The bucket owner automatically owns and has full control over every object in the bucket. The bucket only accepts PUT requests that don't specify an ACL or specify bucket owner full control ACLs (such as the predefined bucket-owner-full-control canned ACL or a custom ACL in XML format that grants the same permissions).</summary>
    BucketOwnerEnforced
}