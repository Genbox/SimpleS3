using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

[FastEnum]
public enum HeaderUsage
{
    Unknown = 0,
    [Display(Name = "NONE")]None,
    [Display(Name = "IGNORE")]Ignore,
    [Display(Name = "USE")]Use
}