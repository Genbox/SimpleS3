using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Optional attributes that can be included in object listing responses.</summary>
[FastEnum]
public enum OptionalObjectAttribute
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,

    /// <summary>Includes restoration status for archived objects.</summary>
    [Display(Name = "RestoreStatus")]
    RestoreStatus
}