using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Abstracts.Features;

/// <summary>Provides the <c>IContentMd5Config</c> API.</summary>
public interface IContentMd5Config : IHasContentMd5
{
    /// <summary>Provides the <c>ForceContentMd5</c> API.</summary>
    Func<bool> ForceContentMd5 { get; }
}