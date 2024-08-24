using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Predefined (canned) Access Control Lists (ACL) for objects</summary>
[FastEnum]
public enum ObjectCannedAcl
{
    Unknown = 0,

    /// <summary>Owner gets full control. No one else has access rights (default).</summary>
    [Display(Name = "private")]
    Private,

    /// <summary>Owner gets full control. The <see cref="PredefinedGroup.AllUsers" /> group gets read access.</summary>
    [Display(Name = "public-read")]
    PublicRead,

    /// <summary>Owner gets full control. The <see cref="PredefinedGroup.AllUsers" /> group gets read and write access. Granting this on a bucket is generally not recommended.</summary>
    [Display(Name = "public-read-write")]
    PublicReadWrite,

    /// <summary>Owner gets full control. Amazon EC2 gets Read access to GET an Amazon Machine Image (AMI) bundle from Amazon S3.</summary>
    [Display(Name = "aws-exec-read")]
    AwsExecRead,

    /// <summary>Owner gets full control. The <see cref="PredefinedGroup.AuthenticatedUsers" /> group gets read access.</summary>
    [Display(Name = "authenticated-read")]
    AuthenticatedRead,

    /// <summary>Object owner gets full control. Bucket owner gets read access.</summary>
    [Display(Name = "bucket-owner-read")]
    BucketOwnerRead,

    /// <summary>Both the object owner and the bucket owner get full control over the object.</summary>
    [Display(Name = "bucket-owner-full-control")]
    BucketOwnerFullControl
}