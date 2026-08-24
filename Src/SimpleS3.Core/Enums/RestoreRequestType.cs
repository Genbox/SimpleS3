using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Provides the <c>RestoreRequestType</c> API.</summary>
[FastEnum]
public enum RestoreRequestType
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>Select</c> API.</summary>
    [Display(Name = "SELECT")]Select
}