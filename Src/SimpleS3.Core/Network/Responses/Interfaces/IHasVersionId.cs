namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

/// <summary>Provides the <c>IHasVersionId</c> API.</summary>
public interface IHasVersionId
{
    /// <summary>Provides the <c>VersionId</c> API.</summary>
    string? VersionId { get; }
}