using Genbox.SimpleS3.Core.Abstracts.Response;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

/// <summary>Provides the <c>IHasContent</c> API.</summary>
[PublicAPI]
public interface IHasContent : IDisposable
{
    /// <summary>Provides the <c>Content</c> API.</summary>
    ContentStream Content { get; }
}