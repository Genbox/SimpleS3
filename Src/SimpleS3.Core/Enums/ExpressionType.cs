using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

[FastEnum]
public enum ExpressionType
{
    Unknown = 0,
    [Display(Name = "SQL")]Sql
}