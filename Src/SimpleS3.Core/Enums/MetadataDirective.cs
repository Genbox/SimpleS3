using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

[FastEnum]
public enum MetadataDirective
{
    Unknown = 0,
    [Display(Name = "COPY")]Copy,
    [Display(Name = "REPLACE")]Replace
}