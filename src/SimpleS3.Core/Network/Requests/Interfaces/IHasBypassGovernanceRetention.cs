namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces;

public interface IHasBypassGovernanceRetention
{
    /// <summary>
    /// Specifies whether you want to delete this object even if it has a Governance-type Object Lock in place. You must have sufficient permissions
    /// to perform this operation.
    /// </summary>
    bool? BypassGovernanceRetention { get; set; }
}