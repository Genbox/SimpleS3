using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>
/// Indicates the checksum type that you want Amazon S3 to use to calculate the object’s checksum value. For more information, see
/// https://docs.aws.amazon.com/AmazonS3/latest/userguide/checking-object-integrity.html
/// </summary>
[FastEnum]
public enum ChecksumType
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>Composite</c> API.</summary>
    [Display(Name = "COMPOSITE")]Composite,
    /// <summary>Provides the <c>FullObject</c> API.</summary>
    [Display(Name = "FULL_OBJECT")]FullObject
}