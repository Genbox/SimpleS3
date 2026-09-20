using Genbox.HttpBuilders.Abstracts;
using Genbox.SimpleS3.Core.Common.Validation;
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

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public void Reset() => _dict?.Clear();

    /// <summary>Provides the <c>HasData</c> API.</summary>
    public bool HasData() => _dict != null && _dict.Count > 0;

    /// <summary>Provides the <c>AddEntry</c> API.</summary>
    public void AddEntry(string key, string value)
    {
        Validator.RequireNotNull(key);
        Validator.RequireNotNull(value);

        _dict ??= new Dictionary<string, string>(StringComparer.Ordinal);

        if (!Valid(key))
            throw new ArgumentException("Invalid character in key. Only digits, letters and the follow characters are allowed _, -, /, \\, :", nameof(key));

        if (!Valid(value))
            throw new ArgumentException("Invalid character in value. Only digits, letters and the follow characters are allowed _, -, /, \\, :", nameof(value));

        _dict.Add(key, value);
    }

    private static bool Valid(string item)
    {
        foreach (char c in item)
        {
            if (c is '_' or '-' or '/' or '\\' or ':')
                continue;

            if (char.IsLetterOrDigit(c))
                continue;

            return false;
        }

        return true;
    }
}