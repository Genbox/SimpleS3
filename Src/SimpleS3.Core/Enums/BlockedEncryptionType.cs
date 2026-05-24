using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

[FastEnum]
public enum BlockedEncryptionType
{
    Unknown = 0,
    [Display(Name = "NONE")]None,
    [Display(Name = "SSE-C")]SseC
}
