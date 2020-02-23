using System;
using System.Globalization;
using System.Text;
using System.Xml;
using Genbox.SimpleS3.Core.Common.Helpers;

namespace Genbox.SimpleS3.Core.Internals.Xml
{
    /// <summary>A fast optimized XML 1.1 standard compliant writer that does not allocate more memory than absolutely needed.</summary>
    internal class FastXmlWriter
    {
        private readonly XmlStandard _xmlStandard;
        private readonly XmlCharMode _invalidCharMode;
        private readonly XmlCharMode _discouragedCharMode;
        private readonly StringBuilder _xml;

        /// <summary>Create a new <see cref="FastXmlWriter" /> with the specified capacity.</summary>
        /// <param name="capacity">The capacity of the XML writer. Note that it does not expand any further.</param>
        /// <param name="xmlStandard">The XML standard to use</param>
        /// <param name="invalidChars">What should the writer do when it encounters an invalid character</param>
        /// <param name="discouragedChars">What should the writer do when it encounters a discouraged character</param>
        public FastXmlWriter(int capacity, XmlStandard xmlStandard = XmlStandard.Xml10, XmlCharMode invalidChars = XmlCharMode.ThrowException, XmlCharMode discouragedChars = XmlCharMode.ThrowException)
        {
            if (invalidChars == XmlCharMode.EntityEncode)
                throw new ArgumentException("EntityEncode mode is not supported for invalid XML characters");

            _xmlStandard = xmlStandard;
            _invalidCharMode = invalidChars;
            _discouragedCharMode = discouragedChars;


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
        /// <param name="xmlns">Namespace to include in the element (if any)</param>
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
            // According to https://www.w3.org/TR/xml11/#syntax <, & and > must be escaped
            // According to https://www.unicode.org/versions/Unicode12.0.0/UnicodeStandard-12.0.pdf section 2.4 0xFDD0-0xFDEF, 0xFFFE and 0xFFFF are non-characters

            for (int i = 0; i < data.Length; i++)
            {
                char c = data[i];

                if (c == '&')
                    _xml.Append("&amp;");
                else if (c == '<')
                    _xml.Append("&lt;");
                else if (c == '>')
                    _xml.Append("&gt;");
                else if (char.IsHighSurrogate(c) && i + 1 < data.Length && char.IsLowSurrogate(data[i + 1]))
                {
                    _xml.Append(c);
                    _xml.Append(data[++i]);
                }
                else if (IsValidChar(c))
                {
                    if (IsDiscouraged(c))
                    {
                        switch (_discouragedCharMode)
                        {
                            case XmlCharMode.EntityEncode:
                                //Note: Entity encoding might only be valid for discouraged characters in XML 1.1
                                _xml.Append("&#x");
                                _xml.Append(Convert.ToString(c, 16));
                                _xml.Append(';');
                                continue;
                            case XmlCharMode.Omit:
                                continue;
                            case XmlCharMode.ThrowException:
                                throw new XmlException($"Discouraged XML character: 0x{Convert.ToString(c, 16)} at offset {i}");
                        }
                    }

                    _xml.Append(c);
                }
                else
                {
                    switch (_invalidCharMode)
                    {
                        case XmlCharMode.Omit:
                            continue;
                        case XmlCharMode.ThrowException:
                            throw new XmlException($"Invalid XML character: 0x{Convert.ToString(c, 16)} at offset {i}");
                    }
                }
            }
        }

        private bool IsValidChar(char c)
        {
            // According to https://www.w3.org/TR/xml/#charsets #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF] are valid
            if (_xmlStandard == XmlStandard.Xml10)
            {
                return c == '\u0009'
                    || c == '\u000A'
                    || c == '\u000D'
                    || CharHelper.InRange(c, '\u0020', '\uD7FF')
                    || CharHelper.InRange(c, '\uE000', '\uFFFD');
            }

            // According to https://www.w3.org/TR/xml11/#charsets [#x1-#xD7FF] | [#xE000-#xFFFD] are valid characters
            if (_xmlStandard == XmlStandard.Xml11)
            {
                return CharHelper.InRange(c, '\u0001', '\uD7FF')
                    || CharHelper.InRange(c, '\uE000', '\uFFFD');
            }

            throw new Exception("Invalid XML standard");
        }

        private bool IsDiscouraged(char c)
        {
            // According to https://www.w3.org/TR/xml/#charsets [#x7F-#x84], [#x86-#x9F], [#xFDD0-#xFDEF] are discouraged
            if (_xmlStandard == XmlStandard.Xml10)
            {
                return CharHelper.InRange(c, '\u007F', '\u0084')
                    || CharHelper.InRange(c, '\u0086', '\u009F')
                    || CharHelper.InRange(c, '\uFDD0', '\uFDEF'); //Unicode non-characters
            }

            // According to https://www.w3.org/TR/xml11/#charsets [#x1-#x8] | [#xB-#xC] | [#xE-#x1F] | [#x7F-#x84] | [#x86-#x9F] are discouraged
            if (_xmlStandard == XmlStandard.Xml11)
            {
                return CharHelper.InRange(c, '\u0001', '\u0008')
                    || CharHelper.InRange(c, '\u000B', '\u000C')
                    || CharHelper.InRange(c, '\u000E', '\u001F')
                    || CharHelper.InRange(c, '\u007F', '\u0084')
                    || CharHelper.InRange(c, '\u0086', '\u009F')
                    || CharHelper.InRange(c, '\uFDD0', '\uFDEF'); //Unicode non-characters
            }

            throw new Exception("Invalid XML standard");
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