using System.Runtime.Serialization;
using Genbox.SimpleS3.Abstracts.Enums;

namespace Genbox.SimpleS3.Core.Enums
{
    /// <summary>Predefined (canned) Access Control Lists (ACL) for buckets</summary>
    public enum BucketCannedAcl
    {
        Unknown = 0,

        /// <summary>Owner gets full control. No one else has access rights (default).</summary>
        [EnumMember(Value = "private")]
        Private,

        /// <summary>Owner gets full control. The <see cref="PredefinedGroup.AllUsers" /> group gets read access.</summary>
        [EnumMember(Value = "public-read")]
        PublicRead,

        /// <summary>
        /// Owner gets full control. The <see cref="PredefinedGroup.AllUsers" /> group gets read and write access. Granting this on a bucket is
        /// generally not recommended.
        /// </summary>
        [EnumMember(Value = "public-read-write")]
        PublicReadWrite,

        /// <summary>Owner gets full control. Amazon EC2 gets Read access to GET an Amazon Machine Image (AMI) bundle from Amazon S3.</summary>
        [EnumMember(Value = "aws-exec-read")]
        AwsExecRead,

        /// <summary>Owner gets full control. The <see cref="PredefinedGroup.AuthenticatedUsers" /> group gets read access.</summary>
        [EnumMember(Value = "authenticated-read")]
        AuthenticatedRead,

        /// <summary>The <see cref="PredefinedGroup.LogDelivery" /> group gets write and read ACL permissions on the bucket.</summary>
        [EnumMember(Value = "log-delivery-write")]
        LogDeliveryWrite
    }
}