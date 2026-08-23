using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

[FastEnum]
public enum AnnotationTableConfigurationState
{
    Unknown = 0,
    [Display(Name = "ENABLED")]Enabled,
    [Display(Name = "DISABLED")]Disabled
}