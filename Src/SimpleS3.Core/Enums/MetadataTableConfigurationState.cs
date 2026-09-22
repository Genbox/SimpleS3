using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>State of an S3 Metadata table configuration.</summary>
[FastEnum]
public enum MetadataTableConfigurationState
{
    /// <summary>Unknown state.</summary>
    Unknown = 0,
    /// <summary>Enabled.</summary>
    [Display(Name = "ENABLED")]
    Enabled,
    /// <summary>Disabled.</summary>
    [Display(Name = "DISABLED")]
    Disabled
}