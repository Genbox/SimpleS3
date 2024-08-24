using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

[FastEnum]
public enum CompressionType
{
    Unknown = 0,
    [Display(Name = "NONE")]None,
    [Display(Name = "GZIP")]Gzip,
    [Display(Name = "BZIP2")]Bzip
}