using System.Diagnostics.CodeAnalysis;
using Genbox.HttpBuilders.Abstracts;
using Genbox.SimpleS3.Core.Internals.Helpers;

namespace Genbox.SimpleS3.Core.Builders;

/// <summary>Provides the <c>KmsContextBuilder</c> API.</summary>
public class KmsContextBuilder : IHttpHeaderBuilder
{
    private Dictionary<string, string>? _dict;

    /// <summary>Provides the <c>KmsContextBuilder</c> API.</summary>
    public KmsContextBuilder(IDictionary<string, string>? dict = null)
    {
        if (dict == null)
            return;

        foreach (KeyValuePair<string, string> pair in dict)
            AddEntry(pair.Key, pair.Value);
    }

    /// <summary>Provides the <c>HeaderName</c> API.</summary>
    public string? HeaderName => null;

    /// <summary>Provides the <c>Build</c> API.</summary>
    public string? Build()
    {
        if (!HasData())
            return null;

        return JsonHelper.EncodeJson(_dict!);
    }

    [SuppressMessage("Design", "CA1033", Justification = "Reset remains an implementation detail and is exposed only through the external header-builder contract.")]
    void IHttpHeaderBuilder.Reset() => Reset();

    /// <summary>Provides the <c>HasData</c> API.</summary>
    public bool HasData() => _dict != null && _dict.Count > 0;

    internal void Reset() => _dict?.Clear();

    /// <summary>Provides the <c>AddEntry</c> API.</summary>
    public void AddEntry(string key, string value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        _dict ??= new Dictionary<string, string>(StringComparer.Ordinal);
        _dict.Add(key, value);
    }
}