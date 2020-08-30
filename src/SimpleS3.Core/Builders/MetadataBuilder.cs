using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genbox.SimpleS3.Core.Internals.Pools;

namespace Genbox.SimpleS3.Core.Builders
{
    public class MetadataBuilder : IEnumerable<KeyValuePair<string, string>>, IPooledObject
    {
        private const string _metadataHeader = "x-amz-meta-";
        private readonly ISet<byte> _allowed;
        private IDictionary<string, string>? _metadata;
        private int _totalSize;

        public MetadataBuilder()
        {
            _allowed = new HashSet<byte>( /*95*/);

            //Determined by brute-force against amazon's S3 service
            byte[] bytes = Encoding.UTF8.GetBytes("! \"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~");

            foreach (byte b in bytes)
                _allowed.Add(b);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            if (_metadata == null)
                return Enumerable.Empty<KeyValuePair<string, string>>().GetEnumerator();

            return _metadata.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
                yield return new KeyValuePair<string, string>(_metadataHeader + item.Key, item.Value);
        }

        public void Reset()
        {
            _metadata?.Clear();
        }
    }
}