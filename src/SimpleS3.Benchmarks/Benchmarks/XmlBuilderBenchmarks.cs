using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using BenchmarkDotNet.Attributes;
using Genbox.SimpleS3.Core.Internal.Xml;

namespace Genbox.SimpleS3.Benchmarks.Benchmarks
{
    [InProcess]
    [MemoryDiagnoser]
    public class XmlBuilderBenchmarks
    {
        [Benchmark]
        public string StringBuilderTest()
        {
            string s = null;
            for (int i = 0; i < 1000; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<rootnode>");
                sb.Append("<person>");
                sb.Append("<name>").Append("santa claus").Append("</name>");
                sb.Append("<age>").Append("800").Append("</age>");
                sb.Append("<status>").Append("missing").Append("</status>");
                sb.Append("</person>");
                sb.Append("</rootnode>");
                s = sb.ToString();
            }

            return s;
        }

        [Benchmark]
        public string XmlTextWriterTest()
        {
            string s = null;
            for (int i = 0; i < 1000; i++)
            {
                using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                using (XmlTextWriter writer = new XmlTextWriter(sw))
                {
                    writer.WriteStartElement("rootnode");
                    writer.WriteStartElement("person");

                    writer.WriteElementString("name", "santa claus");
                    writer.WriteElementString("age", "800");
                    writer.WriteElementString("status", "missing");

                    writer.WriteEndElement();
                    writer.WriteEndElement();

                    s = sw.ToString();
                }
            }

            return s;
        }

        [Benchmark]
        public string XmlTextWriterIndirectionTest()
        {
            string s = null;
            for (int i = 0; i < 1000; i++)
            {
                using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
                using (XmlWriter writer = XmlWriter.Create(sw))
                {
                    writer.WriteStartElement("rootnode");
                    writer.WriteStartElement("person");

                    writer.WriteElementString("name", "santa claus");
                    writer.WriteElementString("age", "800");
                    writer.WriteElementString("status", "missing");

                    writer.WriteEndElement();
                    writer.WriteEndElement();

                    s = sw.ToString();
                }
            }

            return s;
        }

        [Benchmark]
        public string FastXmlWriterTest()
        {
            string s = null;
            for (int i = 0; i < 1000; i++)
            {
                FastXmlWriter writer = new FastXmlWriter(100);
                writer.WriteStartElement("rootnode");
                writer.WriteStartElement("person");

                writer.WriteElement("name", "santa claus");
                writer.WriteElement("age", "800");
                writer.WriteElement("status", "missing");

                writer.WriteEndElement("person");
                writer.WriteEndElement("rootnode");

                s = writer.ToString();
            }

            return s;
        }
    }
}