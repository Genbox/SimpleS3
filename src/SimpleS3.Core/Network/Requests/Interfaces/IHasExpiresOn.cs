namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces;

public interface IHasExpiresOn
{
    /// <summary>The date and time at which the object is no longer able to be cached.</summary>
    DateTimeOffset? ExpiresOn { get; set; }
}