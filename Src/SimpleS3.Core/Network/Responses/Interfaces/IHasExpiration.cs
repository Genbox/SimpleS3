namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

/// <summary>Provides the <c>IHasExpiration</c> API.</summary>
public interface IHasExpiration
{
    /// <summary>The datetime the object expires</summary>
    DateTimeOffset? LifeCycleExpiresOn { get; }

    /// <summary>The rule id used for expiration</summary>
    string? LifeCycleRuleId { get; }
}