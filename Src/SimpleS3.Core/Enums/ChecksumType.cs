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
    Unknown = 0,
    [Display(Name = "COMPOSITE")]Composite,
    [Display(Name = "FULL_OBJECT")]FullObject
}