using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

[FastEnum]
public enum JsonType
{
    Unknown = 0,
    [Display(Name = "DOCUMENT")]Document,
    [Display(Name = "LINES")]Lines
}