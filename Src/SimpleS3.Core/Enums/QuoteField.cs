using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Provides the <c>QuoteField</c> API.</summary>
[FastEnum]
public enum QuoteField
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>Always</c> API.</summary>
    [Display(Name = "ALWAYS")]Always,
    /// <summary>Provides the <c>AsNeeded</c> API.</summary>
    [Display(Name = "ASNEEDED")]AsNeeded
}