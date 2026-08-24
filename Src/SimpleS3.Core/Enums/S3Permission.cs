using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Provides the <c>S3Permission</c> API.</summary>
[FastEnum]
public enum S3Permission
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>FullControl</c> API.</summary>
    [Display(Name = "FULL_CONTROL")]FullControl,
    /// <summary>Provides the <c>Read</c> API.</summary>
    [Display(Name = "READ")]Read,
    /// <summary>Provides the <c>Write</c> API.</summary>
    [Display(Name = "WRITE")]Write
}