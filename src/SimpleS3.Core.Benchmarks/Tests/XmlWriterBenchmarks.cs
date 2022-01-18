using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using BenchmarkDotNet.Attributes;
using Genbox.SimpleS3.Core.Internals.Xml;

namespace Genbox.SimpleS3.Core.Benchmarks.Tests;

[InProcess]
[MemoryDiagnoser]
public class XmlWriterBenchmarks
{
    [Benchmark]
    public string StringBuilderTest()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<rootnode>");
        sb.Append("<person>");
        sb.Append("<name>").Append("santa claus").Append("</name>");
        sb.Append("<age>").Append("800").Append("</age>");
        sb.Append("<status>").Append("missing").Append("</status>");
        sb.Append("<name>").Append("Donald 💩💩💩💩💩\0\0\0\0").Append("</name>");
        sb.Append("<age>").Append("7").Append("</age>");
        sb.Append("<status>").Append("present").Append("</status>");
        sb.Append("</person>");
        sb.Append("</rootnode>");
        return sb.ToString();
    }

    [Benchmark]
    public string XmlTextWriterTest()
    {
        using (StringWriter sw = new StringWriter(CultureInfo.InvariantCulture))
        using (XmlTextWriter writer = new XmlTextWriter(sw))
        {
            writer.WriteStartElement("rootnode");
            writer.WriteStartElement("person");

            writer.WriteElementString("name", "santa claus");
            writer.WriteElementString("age", "800");
            writer.WriteElementString("status", "missing");

            writer.WriteElementString("name", "Donald 💩💩💩💩💩\0\0\0\0");
            writer.WriteElementString("age", "7");
            writer.WriteElementString("status", "present");

            writer.WriteEndElement();
            writer.WriteEndElement();

            return sw.ToString();
        }
    }

    [Benchmark]
    public string FastXmlWriterTest()
    {
        FastXmlWriter writer = new FastXmlWriter(100);
        writer.WriteStartElement("rootnode");
        writer.WriteStartElement("person");

        writer.WriteElement("name", "santa claus");
        writer.WriteElement("age", "800");
        writer.WriteElement("status", "missing");

        writer.WriteElement("name", "Donald 💩💩💩💩💩\0\0\0\0");
        writer.WriteElement("age", "7");
        writer.WriteElement("status", "present");

        writer.WriteEndElement("person");
        writer.WriteEndElement("rootnode");

        return writer.GetXmlString();
    }
}