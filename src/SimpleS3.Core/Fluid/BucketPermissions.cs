using System;

namespace Genbox.SimpleS3.Core.Fluid
{
    [Flags]
    public enum BucketPermissions
    {
        Unknown = 0,

        /// <summary>Equivalent to s3:ListBucket, s3:ListBucketVersions, and s3:ListBucketMultipartUploads</summary>
        Read = 1,

        /// <summary>Equivalent to s3:PutObject and s3:DeleteObject.</summary>
        Write = 2,

        /// <summary>Equivalent tos3:GetBucketAcl</summary>
        ReadAcl = 4,

        /// <summary>Equivalent to s3:PutBucketAcl</summary>
        WriteAcl = 8,

        /// <summary>Equivalent to granting Read, Write, ReadAcl, and WriteAcl permissions.</summary>
        FullControl = 16
    }
}