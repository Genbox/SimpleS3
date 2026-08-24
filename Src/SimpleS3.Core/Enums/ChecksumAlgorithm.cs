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
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>Crc32</c> API.</summary>
    [Display(Name = "CRC32")]Crc32,
    /// <summary>Provides the <c>Crc32C</c> API.</summary>
    [Display(Name = "CRC32C")]Crc32C,
    /// <summary>Provides the <c>Sha1</c> API.</summary>
    [Display(Name = "SHA1")]Sha1,
    /// <summary>Provides the <c>Sha256</c> API.</summary>
    [Display(Name = "SHA256")]Sha256,
    /// <summary>Provides the <c>Crc64Nvme</c> API.</summary>
    [Display(Name = "CRC64NVME")]Crc64Nvme,
    /// <summary>Provides the <c>Sha512</c> API.</summary>
    [Display(Name = "SHA512")]Sha512,
    /// <summary>Provides the <c>Md5</c> API.</summary>
    [Display(Name = "MD5")]Md5,
    /// <summary>Provides the <c>XxHash64</c> API.</summary>
    [Display(Name = "XXHASH64")]XxHash64,
    /// <summary>Provides the <c>XxHash3</c> API.</summary>
    [Display(Name = "XXHASH3")]XxHash3,
    /// <summary>Provides the <c>XxHash128</c> API.</summary>
    [Display(Name = "XXHASH128")]XxHash128
}