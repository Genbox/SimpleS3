namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

/// <summary>Provides the <c>IHasExpiresOn</c> API.</summary>
public interface IHasExpiresOn
{
    /// <summary>A date time for when the response has expired and should no longer be cached.</summary>
    DateTimeOffset? ExpiresOn { get; }
}