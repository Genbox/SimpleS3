namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

/// <summary>Provides the <c>IHasLastModified</c> API.</summary>
public interface IHasLastModified
{
    /// <summary>Provides the <c>LastModified</c> API.</summary>
    DateTimeOffset? LastModified { get; }
}