using System;
using System.Globalization;
using System.Text;
using Genbox.SimpleS3.Utils.Helpers;

namespace Genbox.SimpleS3.Core.Internal.Xml
{
    /// <summary>A fast optimized XML writer that does not allocate more memory than absolutely needed.</summary>
    public class FastXmlWriter
    {
        private readonly StringBuilder _xml;

        /// <summary>Create a new <see cref="FastXmlWriter" /> with the specified capacity.</summary>
        /// <param name="capacity">The capacity of the XML writer. Note that it does not expand any further.</param>
        public FastXmlWriter(int capacity)
        {
            _xml = new StringBuilder(capacity);
        }

        /// <summary>Write a new element with the specified value</summary>
        /// <param name="name">The element name</param>
        /// <param name="value">The value within the element</param>
        public void WriteElement(in string name, in string value)
        {
            WriteStartElement(name);
            WriteValue(value);
            WriteEndElement(name);
        }

        /// <summary>Write a new element with the specified value</summary>
        /// <param name="name">The element name</param>
        /// <param name="value">The value within the element</param>
        public void WriteElement(in string name, in int value)
        {
            WriteStartElement(name);
            WriteValue(value.ToString(NumberFormatInfo.InvariantInfo));
            WriteEndElement(name);
        }

        /// <summary>Write a new element with the specified value</summary>
        /// <param name="name">The element name</param>
        /// <param name="value">The value within the element</param>
        public void WriteElement(in string name, in bool value)
        {
            WriteStartElement(name);
            WriteValue(value.ToString(NumberFormatInfo.InvariantInfo));
            WriteEndElement(name);
        }

        public void WriteElement(in string name, in bool? value)
        {
            WriteStartElement(name);
            WriteValue(value.Value.ToString(NumberFormatInfo.InvariantInfo));
            WriteEndElement(name);
        }

        /// <summary>Write the start of an element</summary>
        /// <param name="name">Name of the element</param>
        public void WriteStartElement(in string name, in string xmlns = null)
        {
            _xml.Append('<');
            _xml.Append(name);

            if (xmlns != null)
                _xml.Append(" xmlns=\"").Append(xmlns).Append("\"");

            _xml.Append('>');
        }

        /// <summary>Write the end of an element</summary>
        /// <param name="name">Name of the element</param>
        public void WriteEndElement(in string name)
        {
            _xml.Append("</");
            _xml.Append(name);
            _xml.Append('>');
        }

        private void WriteValue(in string data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                char c = data[i];

                if (c == '<')
                    _xml.Append("&lt;");
                else if (c == '>')
                    _xml.Append("&gt;");
                else if (c == '&')
                    _xml.Append("&amp;");
                else if (CharHelper.InRange(c, '\u0020', '\u007E') //Basic latin
                         || c == '\u0009' //Tab
                         || c == '\u000A' //Line feed
                         || c == '\u000D' //Carriage return
                         || c == '\u0085' //Next line
                         || CharHelper.InRange(c, '\u00A0', '\uD7FF')
                         || CharHelper.InRange(c, '\uE000', '\uFDCF')
                         || CharHelper.InRange(c, '\uFDF0', '\uFFFD'))
                {
                    //See https://www.w3.org/TR/xml/#charsets
                    _xml.Append(c);
                }
                else if (char.IsHighSurrogate(c))
                {
                    if (i + 1 >= data.Length)
                        continue;

                    _xml.Append(c);
                    _xml.Append(data[++i]);
                }
                else
                {
                    _xml.Append("&#x");
                    _xml.Append(Convert.ToString(c, 16));
                    _xml.Append(';');
                }
            }
        }

        /// <summary>Return a set of UTF8 encoded bytes</summary>
        public byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes(_xml.ToString());
        }

        /// <summary>Gives a string representation of the XML structure</summary>
        public override string ToString()
        {
            return _xml.ToString();
        }

        public void Clear()
        {
            _xml.Clear();
        }
    }
}