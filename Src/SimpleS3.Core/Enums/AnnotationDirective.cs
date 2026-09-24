using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Controls whether annotations are copied with an object.</summary>
[FastEnum]
public enum AnnotationDirective
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Copy annotations from the source object.</summary>
    [Display(Name = "COPY")]Copy,
    /// <summary>Exclude annotations from the copy.</summary>
    [Display(Name = "EXCLUDE")]Exclude
}