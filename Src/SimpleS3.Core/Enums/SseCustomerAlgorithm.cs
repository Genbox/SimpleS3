using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Provides the <c>SseCustomerAlgorithm</c> API.</summary>
[FastEnum]
public enum SseCustomerAlgorithm
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>Aes256</c> API.</summary>
    [Display(Name = "AES256")]Aes256
}