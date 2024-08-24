using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

[FastEnum]
public enum ReplicationStatus
{
    Unknown = 0,
    [Display(Name = "PENDING")]Pending,
    [Display(Name = "COMPLETED")]Completed,
    [Display(Name = "FAILED")]Failed,
    [Display(Name = "REPLICA")]Replica
}