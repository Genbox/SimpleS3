using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

[FastEnum]
public enum RestoreRequestType
{
    Unknown = 0,
    [Display(Name = "SELECT")]Select
}