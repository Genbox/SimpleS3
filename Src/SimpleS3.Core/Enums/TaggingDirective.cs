using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Provides the <c>TaggingDirective</c> API.</summary>
[FastEnum]
public enum TaggingDirective
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>Copy</c> API.</summary>
    [Display(Name = "COPY")]Copy,
    /// <summary>Provides the <c>Replace</c> API.</summary>
    [Display(Name = "REPLACE")]Replace
}