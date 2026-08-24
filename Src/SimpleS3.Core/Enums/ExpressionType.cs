using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Provides the <c>ExpressionType</c> API.</summary>
[FastEnum]
public enum ExpressionType
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>Sql</c> API.</summary>
    [Display(Name = "SQL")]Sql
}