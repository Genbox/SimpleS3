using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Used to specify the encoding of responses when using certain APIs that are based on XML.</summary>
[FastEnum]
public enum EncodingType
{
    Unknown = 0,
    [Display(Name = "url")]Url
}