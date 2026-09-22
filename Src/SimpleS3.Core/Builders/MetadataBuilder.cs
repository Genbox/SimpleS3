using System.Collections;
using System.Text;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Pools;

namespace Genbox.SimpleS3.Core.Builders;

/// <summary>Provides the <c>MetadataBuilder</c> API.</summary>
public class MetadataBuilder : IEnumerable<KeyValuePair<string, string>>, IPooledObject
{
    private Dictionary<string, string>? _metadata;
    private int _totalSize;

    //Determined by brute-force against amazon's S3 service
    //! "#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~

    /// <summary>Provides the <c>GetEnumerator</c> API.</summary>
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        if (_metadata == null)
            return Enumerable.Empty<KeyValuePair<string, string>>().GetEnumerator();

        return _metadata.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Provides the <c>Reset</c> API.</summary>
    void IPooledObject.Reset() => Reset();

    internal void Reset()
    {
        _metadata?.Clear();
        _totalSize = 0;
    }

    /// <summary>Provides the <c>Add</c> API.</summary>
    public MetadataBuilder Add(string key, string value)
    {
        //From the docs: Within the PUT request header, the user-defined metadata is limited to 2 KB in size.
        //The size of user-defined metadata is measured by taking the sum of the number of bytes in the UTF-8 encoding of each key and value.

        _metadata ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrEmpty(key) || key.Any(c => !IsHeaderNameCharacter(c)))
            throw new ArgumentException("Metadata keys must contain only HTTP header token characters.", nameof(key));

        if (value.Any(c => c is < ' ' or > '~'))
            throw new ArgumentException("Metadata values must contain only printable ASCII characters.", nameof(value));

        int totalSize = checked(_totalSize + Encoding.UTF8.GetByteCount(key) + Encoding.UTF8.GetByteCount(value));

        if (totalSize > 2048)
            throw new ArgumentException("Metadata too large", nameof(value));

        _metadata.Add(key, value);
        _totalSize = totalSize;

        return this;
    }

    internal IEnumerable<KeyValuePair<string, string>> GetPrefixed()
    {
        foreach (KeyValuePair<string, string> item in this)
            yield return new KeyValuePair<string, string>(Constants.AmzMetadata + item.Key, item.Value);
    }

    private static bool IsHeaderNameCharacter(char c) => c is >= 'a' and <= 'z'
        or >= 'A' and <= 'Z'
        or >= '0' and <= '9'
        or '!' or '#' or '$' or '%' or '&' or '\'' or '*' or '+' or '-' or '.' or '^' or '_' or '`' or '|' or '~';
}