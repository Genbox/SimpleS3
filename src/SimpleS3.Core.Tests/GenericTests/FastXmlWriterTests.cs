using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Genbox.SimpleS3.Core.Internals.Xml;
using Xunit;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class FastXmlWriterTests
{
    [Theory]
    [InlineData(XmlStandard.Xml10)]
    [InlineData(XmlStandard.Xml11)]
    internal void EncodeSyntaxCharacters(XmlStandard standard)
    {
        FastXmlWriter writer = new FastXmlWriter(50, standard);
        writer.WriteElement("Test", "<&>");

        Assert.Equal("<Test>&lt;&amp;&gt;</Test>", Encoding.UTF8.GetString(writer.GetBytes()));
    }

    [Theory]
    [InlineData(XmlStandard.Xml10)]
    [InlineData(XmlStandard.Xml11)]
    internal void EncodeSurrogatePair(XmlStandard standard)
    {
        FastXmlWriter writer = new FastXmlWriter(50, standard);
        writer.WriteElement("Test", "💩");

        string actual = Encoding.UTF8.GetString(writer.GetBytes());

        Assert.Equal("<Test>💩</Test>", actual);
    }

    [Theory]
    [InlineData(XmlStandard.Xml10)]
    [InlineData(XmlStandard.Xml11)]
    internal void EncodeInvalidSurrogateValue(XmlStandard standard)
    {
        FastXmlWriter writer = new FastXmlWriter(50, standard);
        Assert.Throws<XmlException>(() => writer.WriteElement("Test", "Value" + '\uD800')); //standalone high surrogate
    }

    [Theory]
    [InlineData(XmlStandard.Xml10)]
    [InlineData(XmlStandard.Xml11)]
    internal void EncodeNormalValue(XmlStandard standard)
    {
        FastXmlWriter writer = new FastXmlWriter(50, standard);
        writer.WriteElement("Test", "Value");

        Assert.Equal("<Test>Value</Test>", Encoding.UTF8.GetString(writer.GetBytes()));
    }

    [Theory]
    [InlineData(XmlStandard.Xml10)]
    [InlineData(XmlStandard.Xml11)]
    internal void EncodeNullValue(XmlStandard standard)
    {
        FastXmlWriter writer = new FastXmlWriter(50, standard);
        Assert.Throws<XmlException>(() => writer.WriteElement("Test", "Value\0Value"));
    }

    [Theory]
    [InlineData(XmlStandard.Xml10)]
    [InlineData(XmlStandard.Xml11)]
    internal void EncodeDiscouragedValue(XmlStandard standard)
    {
        FastXmlWriter writer = new FastXmlWriter(50, standard);
        Assert.Throws<XmlException>(() => writer.WriteElement("Test", "Value" + '\u007F')); // \u007F is discouraged in both standards
    }

    [Theory]
    [InlineData(XmlStandard.Xml10)]
    [InlineData(XmlStandard.Xml11)]
    internal void XDocumentCompatibility(XmlStandard standard)
    {
        FastXmlWriter writer = new FastXmlWriter(ushort.MaxValue + 200, standard, XmlCharMode.Omit, XmlCharMode.Omit);
        StringBuilder sb = new StringBuilder(ushort.MaxValue);

        for (int i = 0; i < ushort.MaxValue; i++)
        {
            sb.Append((char)i);
        }

        writer.WriteElement("test", sb.ToString());

        using MemoryStream ms = new MemoryStream(writer.GetBytes());
        XDocument _ = XDocument.Load(ms);
    }

    [Theory]
    [InlineData(XmlStandard.Xml10)]
    [InlineData(XmlStandard.Xml11)]
    internal void XmlDocumentCompatibility(XmlStandard standard)
    {
        FastXmlWriter writer = new FastXmlWriter(ushort.MaxValue + 200, standard, XmlCharMode.Omit, XmlCharMode.Omit);
        StringBuilder sb = new StringBuilder(ushort.MaxValue);

        for (int i = 0; i < ushort.MaxValue; i++)
        {
            sb.Append((char)i);
        }

        writer.WriteElement("test", sb.ToString());

        XmlDocument xmlDoc = new XmlDocument();
        using MemoryStream ms = new MemoryStream(writer.GetBytes());
        xmlDoc.Load(ms);
    }
}