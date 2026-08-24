using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Used to specify the encoding of responses when using certain APIs that are based on XML.</summary>
[FastEnum]
public enum EncodingType
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>Url</c> API.</summary>
    [Display(Name = "url")]Url
}