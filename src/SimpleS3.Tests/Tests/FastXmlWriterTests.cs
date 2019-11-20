using System.IO;
using System.Xml;
using Genbox.SimpleS3.Core.Internal.Xml;
using Xunit;

namespace Genbox.SimpleS3.Tests.Tests
{
    public class FastXmlWriterTests
    {
        [Fact]
        public void EncodedXmlValue()
        {
            FastXmlWriter writer = new FastXmlWriter(50);
            writer.WriteElement("Test", "<&>");

            Assert.Equal("<Test>&lt;&amp;&gt;</Test>", writer.ToString());
        }

        [Fact]
        public void ExtendedCharactersXmlValue()
        {
            FastXmlWriter writer = new FastXmlWriter(50);
            writer.WriteElement("Test", "💩");

            string actual = writer.ToString();

            Assert.Equal("<Test>💩</Test>", actual);

            using (StringWriter sw = new StringWriter())
            using (XmlTextWriter xml = new XmlTextWriter(sw))
            {
                xml.WriteElementString("Test", "💩");
                Assert.Equal(sw.ToString(), actual);
            }
        }

        [Fact]
        public void EntityRefXmlValue()
        {
            FastXmlWriter writer = new FastXmlWriter(50);
            writer.WriteElement("Test", "\0");

            string actual = writer.ToString();

            Assert.Equal("<Test>&#x0;</Test>", actual);

            using (StringWriter sw = new StringWriter())
            using (XmlTextWriter xml = new XmlTextWriter(sw))
            {
                xml.WriteElementString("Test", "\0");
                Assert.Equal(sw.ToString(), actual);
            }
        }

        [Fact]
        public void NormalXmlValue()
        {
            FastXmlWriter writer = new FastXmlWriter(50);
            writer.WriteElement("Test", "Value");

            Assert.Equal("<Test>Value</Test>", writer.ToString());
        }
    }
}