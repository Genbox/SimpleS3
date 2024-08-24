using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

[FastEnum]
public enum SseCustomerAlgorithm
{
    Unknown = 0,
    [Display(Name = "AES256")]Aes256
}