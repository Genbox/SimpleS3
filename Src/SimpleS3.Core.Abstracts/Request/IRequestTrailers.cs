using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Abstracts.Request;

/// <summary>Provides the <c>IRequestTrailers</c> API.</summary>
[PublicAPI]
public interface IRequestTrailers
{
    /// <summary>Trailers to apply to the request</summary>
    IReadOnlyDictionary<string, string> Trailers { get; }

    /// <summary>Adds a trailing header</summary>
    /// <param name="key">Name of the trailer. Names are normalized to lowercase.</param>
    /// <param name="value">Value of the trailer</param>
    void SetTrailer(string key, string value);
}