using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

[FastEnum]
public enum S3Permission
{
    Unknown = 0,
    [Display(Name = "FULL_CONTROL")]FullControl,
    [Display(Name = "READ")]Read,
    [Display(Name = "WRITE")]Write
}