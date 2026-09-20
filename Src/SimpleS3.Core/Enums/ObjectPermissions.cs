namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Provides the <c>ObjectPermissions</c> API.</summary>
[Flags]
public enum ObjectPermissions
{
    /// <summary>Provides the <c>None</c> API.</summary>
    None = 0,

    /// <summary>Equivalent to s3:GetObject and s3:GetObjectVersion</summary>
    Read = 1,

    /// <summary>Equivalent to s3:GetObjectAcl and s3:GetObjectVersionAcl</summary>
    ReadAcl = 2,

    /// <summary>Equivalent to s3:PutObjectAcl and s3:PutObjectVersionAcl</summary>
    WriteAcl = 4,

    /// <summary>Equivalent to granting Read, ReadAcl, and WriteAcl permissions.</summary>
    FullControl = 8
}