namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces;

/// <summary>Provides the <c>IHasLegalHold</c> API.</summary>
public interface IHasLegalHold
{
    /// <summary>Specifies whether a legal hold will be applied to this object.</summary>
    bool? LockLegalHold { get; set; }
}