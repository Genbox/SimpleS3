using Genbox.HttpBuilders;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces;

[PublicAPI]
public interface IHasRange
{
    /// <summary>
    /// Downloads the specified range bytes of an object. For more information about the HTTP Range header, go to
    /// http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.35.
    /// </summary>
    RangeBuilder Range { get; }
}