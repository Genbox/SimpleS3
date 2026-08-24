using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Provides the <c>AnnotationTableConfigurationState</c> API.</summary>
[FastEnum]
public enum AnnotationTableConfigurationState
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>Enabled</c> API.</summary>
    [Display(Name = "ENABLED")]Enabled,
    /// <summary>Provides the <c>Disabled</c> API.</summary>
    [Display(Name = "DISABLED")]Disabled
}