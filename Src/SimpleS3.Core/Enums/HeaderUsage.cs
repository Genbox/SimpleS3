using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Provides the <c>HeaderUsage</c> API.</summary>
[FastEnum]
public enum HeaderUsage
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>None</c> API.</summary>
    [Display(Name = "NONE")]None,
    /// <summary>Provides the <c>Ignore</c> API.</summary>
    [Display(Name = "IGNORE")]Ignore,
    /// <summary>Provides the <c>Use</c> API.</summary>
    [Display(Name = "USE")]Use
}