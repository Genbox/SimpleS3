using System.Collections;
using System.Text;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Pools;

namespace Genbox.SimpleS3.Core.Builders;

public class MetadataBuilder : IEnumerable<KeyValuePair<string, string>>, IPooledObject
{
    private readonly ISet<byte> _allowed;
    private IDictionary<string, string>? _metadata;
    private int _totalSize;

    public MetadataBuilder()
    {
        //Determined by brute-force against amazon's S3 service
        //! "#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
        _allowed = new HashSet<byte> { 33, 32, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126 };
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        if (_metadata == null)
            return Enumerable.Empty<KeyValuePair<string, string>>().GetEnumerator();

        return _metadata.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Reset()
    {
        _metadata?.Clear();
    }

    public MetadataBuilder Add(string key, string value)
    {
        //From the docs: Within the PUT request header, the user-defined metadata is limited to 2 KB in size.
        //The size of user-defined metadata is measured by taking the sum of the number of bytes in the UTF-8 encoding of each key and value.

        if (_metadata == null)
            _metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        byte[] utf8Bytes = Encoding.UTF8.GetBytes(key + value);

        foreach (byte b in utf8Bytes)
        {
            if (!_allowed.Contains(b))
                throw new ArgumentException($"Invalid character '{b}' in metadata");
        }

        _totalSize += utf8Bytes.Length;

        if (_totalSize > 2048)
            throw new ArgumentException("Metadata too large");

        _metadata.Add(key, value);

        return this;
    }

    internal IEnumerable<KeyValuePair<string, string>> GetPrefixed()
    {
        foreach (KeyValuePair<string, string> item in this)
            yield return new KeyValuePair<string, string>(Constants.AmzMetadata + item.Key, item.Value);
    }
}