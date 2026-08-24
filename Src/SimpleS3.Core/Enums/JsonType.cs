using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Provides the <c>JsonType</c> API.</summary>
[FastEnum]
public enum JsonType
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>Document</c> API.</summary>
    [Display(Name = "DOCUMENT")]Document,
    /// <summary>Provides the <c>Lines</c> API.</summary>
    [Display(Name = "LINES")]Lines
}