using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

[FastEnum]
public enum LockMode
{
    Unknown = 0,

    /// <summary>
    /// In governance mode, users can't overwrite or delete an object version or alter its lock settings unless they have special permissions. With governance mode, you protect
    /// objects against being deleted by most users, but you can still grant some users permission to alter the retention settings or delete the object if necessary. You can also use
    /// governance mode to test retention-period settings before creating a compliance-mode retention period. To override or remove governance-mode retention settings, a user must have
    /// the s3:BypassGovernanceRetention permission and must explicitly include x-amz-bypass-governance-retention:true as a request header with any request that requires overriding
    /// governance mode.
    /// </summary>
    [Display(Name = "GOVERNANCE")]
    Governance,

    /// <summary>
    /// In compliance mode, a protected object version can't be overwritten or deleted by any user, including the root user in your AWS account. When an object is locked in
    /// compliance mode, its retention mode can't be changed, and its retention period can't be shortened. Compliance mode ensures that an object version can't be overwritten or deleted
    /// for the duration of the retention period.
    /// </summary>
    [Display(Name = "COMPLIANCE")]
    Compliance
}