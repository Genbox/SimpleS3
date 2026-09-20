using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Provides the <c>BlockedEncryptionType</c> API.</summary>
[FastEnum]
public enum BlockedEncryptionType
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>None</c> API.</summary>
    [Display(Name = "NONE")]None,
    /// <summary>Provides the <c>SseC</c> API.</summary>
    [Display(Name = "SSE-C")]
    SseC
}