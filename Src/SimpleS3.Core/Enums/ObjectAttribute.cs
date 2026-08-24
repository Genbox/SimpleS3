using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Provides the <c>ObjectAttribute</c> API.</summary>
[FastEnum]
public enum ObjectAttribute
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>ETag</c> API.</summary>
    [Display(Name = "ETag")]ETag,
    /// <summary>Provides the <c>Checksum</c> API.</summary>
    [Display(Name = "Checksum")]Checksum,
    /// <summary>Provides the <c>ObjectParts</c> API.</summary>
    [Display(Name = "ObjectParts")]ObjectParts,
    /// <summary>Provides the <c>StorageClass</c> API.</summary>
    [Display(Name = "StorageClass")]StorageClass,
    /// <summary>Provides the <c>ObjectSize</c> API.</summary>
    [Display(Name = "ObjectSize")]ObjectSize
}