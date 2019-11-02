using System;

namespace Genbox.SimpleS3.Core.Network.Requests.Properties
{
    public interface IHasExpires
    {
        /// <summary>The date and time at which the object is no longer able to be cached.</summary>
        DateTimeOffset? Expires { get; set; }
    }
}