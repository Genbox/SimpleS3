using System;

namespace Genbox.SimpleS3.Core.Network.Responses.Properties
{
    public interface IHasExpiresOn
    {
        /// <summary>A date time for when the response has expired and should no longer be cached.</summary>
        DateTimeOffset? ExpiresOn { get; }
    }
}