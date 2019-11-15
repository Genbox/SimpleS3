using System;
using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Genbox.SimpleS3.Utils.Helpers;

namespace Genbox.SimpleS3.Core.Internal.Xml
{
    public ref struct FastXmlWriter
    {
        private readonly char[] _backing;
        private readonly Span<char> _data;
        private int _position;
        private bool _used;

        public FastXmlWriter(int capacity)
        {
            _backing = ArrayPool<char>.Shared.Rent(capacity);
            _data = new Span<char>(_backing);
            _position = 0;
            _used = false;
        }

        public void WriteElement(ReadOnlySpan<char> name, ReadOnlySpan<char> value)
        {
            if (_used)
                throw new Exception("This instance is already used.");

            _data[_position++] = '<';

            name.CopyTo(_data.Slice(_position, name.Length));
            _position += name.Length;

            _data[_position++] = '>';

            Write(value);

            _data[_position++] = '<';
            _data[_position++] = '/';

            name.CopyTo(_data.Slice(_position, name.Length));
            _position += name.Length;

            _data[_position++] = '>';
        }

        public void WriteStartElement(ReadOnlySpan<char> name)
        {
            if (_used)
                throw new Exception("This instance is already used.");

            _data[_position++] = '<';

            name.CopyTo(_data.Slice(_position, name.Length));
            _position += name.Length;

            _data[_position++] = '>';
        }

        public void WriteEndElement(ReadOnlySpan<char> name)
        {
            if (_used)
                throw new Exception("This instance is already used.");

            _data[_position++] = '<';
            _data[_position++] = '/';

            name.CopyTo(_data.Slice(_position, name.Length));
            _position += name.Length;

            _data[_position++] = '>';
        }

        private void Write(ReadOnlySpan<char> data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                char c = data[i];

                if (c == '<')
                    WriteEntityRefImpl("lt".AsSpan());
                else if (c == '>')
                    WriteEntityRefImpl("gt".AsSpan());
                else if (c == '&')
                    WriteEntityRefImpl("amp".AsSpan());
                else if (IsValidChar(c))
                    _data[_position++] = c;
                else if (char.IsHighSurrogate(c))
                {
                    if (i + 1 < data.Length)
                        WriteSurrogateChar(data[++i], c);
                }
                else
                    WriteCharEntityImpl(c);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsValidChar(char c)
        {
            //See https://www.w3.org/TR/xml/#charsets
            return c == '\u0009' //Tab
                   || c == '\u000A' //Line feed
                   || c == '\u000D' //Carriage return
                   || CharHelper.InRange(c, '\u0020', '\u007E') //Basic latin
                   || c == '\u0085' //Next line
                   || CharHelper.InRange(c, '\u00A0', '\uD7FF')
                   || CharHelper.InRange(c, '\uE000', '\uFDCF')
                   || CharHelper.InRange(c, '\uFDF0', '\uFFFD');
        }

        private void WriteSurrogateChar(char lowChar, char highChar)
        {
            _data[_position++] = highChar;
            _data[_position++] = lowChar;
        }

        private void WriteEntityRefImpl(ReadOnlySpan<char> val)
        {
            _data[_position++] = '&';
            val.CopyTo(_data.Slice(_position, val.Length));
            _position += val.Length;
            _data[_position++] = ';';
        }

        private void WriteCharEntityImpl(ReadOnlySpan<char> val)
        {
            _data[_position++] = '&';
            _data[_position++] = '#';
            _data[_position++] = 'x';
            val.CopyTo(_data.Slice(_position, val.Length));
            _position += val.Length;
            _data[_position++] = ';';
        }

        private void WriteCharEntityImpl(char ch)
        {
            WriteCharEntityImpl(((int)ch).ToString("X", NumberFormatInfo.InvariantInfo).AsSpan());
        }

        public byte[] GetBytes()
        {
            if (_used)
                throw new Exception("This instance is already used.");

            byte[] array = Encoding.UTF8.GetBytes(_data.ToArray());
            _used = true;
            ArrayPool<char>.Shared.Return(_backing);
            return array;
        }

        public override string ToString()
        {
            if (_used)
                throw new Exception("This instance is already used.");

            string str = new string(_data.Slice(0, _position).ToArray());
            _used = true;
            ArrayPool<char>.Shared.Return(_backing);
            return str;
        }
    }
}