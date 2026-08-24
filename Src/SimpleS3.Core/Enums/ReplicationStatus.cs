using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>Provides the <c>ReplicationStatus</c> API.</summary>
[FastEnum]
public enum ReplicationStatus
{
    /// <summary>Provides the <c>Unknown</c> API.</summary>
    Unknown = 0,
    /// <summary>Provides the <c>Pending</c> API.</summary>
    [Display(Name = "PENDING")]Pending,
    /// <summary>Provides the <c>Completed</c> API.</summary>
    [Display(Name = "COMPLETED")]Completed,
    /// <summary>Provides the <c>Complete</c> API.</summary>
    [Display(Name = "COMPLETE")]Complete,
    /// <summary>Provides the <c>Failed</c> API.</summary>
    [Display(Name = "FAILED")]Failed,
    /// <summary>Provides the <c>Replica</c> API.</summary>
    [Display(Name = "REPLICA")]Replica
}