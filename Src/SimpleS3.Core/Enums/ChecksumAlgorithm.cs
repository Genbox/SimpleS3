using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>
/// Indicates the algorithm that you want Amazon S3 to use to create the checksum for the object. For more information, see
/// https://docs.aws.amazon.com/AmazonS3/latest/userguide/checking-object-integrity.html
/// </summary>
[FastEnum]
public enum ChecksumAlgorithm
{
    Unknown = 0,
    [Display(Name = "CRC32")]Crc32,
    [Display(Name = "CRC32C")]Crc32C,
    [Display(Name = "SHA1")]Sha1,
    [Display(Name = "SHA256")]Sha256,
    [Display(Name = "CRC64NVME")]Crc64Nvme
}