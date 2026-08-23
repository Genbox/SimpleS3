using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

[FastEnum]
public enum ObjectAttribute
{
    Unknown = 0,
    [Display(Name = "ETag")]ETag,
    [Display(Name = "Checksum")]Checksum,
    [Display(Name = "ObjectParts")]ObjectParts,
    [Display(Name = "StorageClass")]StorageClass,
    [Display(Name = "ObjectSize")]ObjectSize
}