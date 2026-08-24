using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces;

/// <summary>Provides the <c>IHasContent</c> API.</summary>
[PublicAPI]
public interface IHasContent
{
    /// <summary>Provides the <c>Content</c> API.</summary>
    Stream? Content { get; }
}