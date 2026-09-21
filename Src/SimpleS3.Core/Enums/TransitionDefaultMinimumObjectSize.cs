using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>The default minimum object size for lifecycle transitions when a rule has no explicit size filter.</summary>
[FastEnum]
public enum TransitionDefaultMinimumObjectSize
{
    /// <summary>No setting was supplied or reported.</summary>
    Unknown = 0,

    /// <summary>Objects smaller than 128 KB do not transition to any storage class by default.</summary>
    [Display(Name = "all_storage_classes_128K")]
    AllStorageClasses128K,

    /// <summary>Objects smaller than 128 KB can transition to Glacier Flexible Retrieval or Deep Archive by default.</summary>
    [Display(Name = "varies_by_storage_class")]
    VariesByStorageClass
}