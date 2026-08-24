using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Provides the <c>CompressionType</c> API.</summary>
[FastEnum]
public enum CompressionType
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>None</c> API.</summary>
    [Display(Name = "NONE")]None,
    /// <summary>Provides the <c>Gzip</c> API.</summary>
    [Display(Name = "GZIP")]Gzip,
    /// <summary>Provides the <c>Bzip</c> API.</summary>
    [Display(Name = "BZIP2")]Bzip
}