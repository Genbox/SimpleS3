using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

[FastEnum]
public enum QuoteField
{
    Unknown = 0,
    [Display(Name = "ALWAYS")]Always,
    [Display(Name = "ASNEEDED")]AsNeeded
}