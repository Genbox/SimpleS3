using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>The archive access tier reported for an object in S3 Intelligent-Tiering.</summary>
[FastEnum]
public enum ArchiveStatus
{
    /// <summary>No archive status was reported.</summary>
    Unknown = 0,

    /// <summary>The object is in the Archive Access tier.</summary>
    [Display(Name = "ARCHIVE_ACCESS")]
    ArchiveAccess,

    /// <summary>The object is in the Deep Archive Access tier.</summary>
    [Display(Name = "DEEP_ARCHIVE_ACCESS")]
    DeepArchiveAccess
}