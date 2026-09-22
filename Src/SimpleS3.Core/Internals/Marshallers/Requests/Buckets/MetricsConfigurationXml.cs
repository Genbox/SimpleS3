using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

[SuppressMessage("Security", "S5332", Justification = "The HTTP URI is the namespace mandated by the S3 XML protocol, not a network endpoint.")]
internal static class MetricsConfigurationXml
{
    public static void Write(FastXmlWriter writer, S3MetricsConfiguration configuration)
    {
        writer.WriteStartElement("MetricsConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
        writer.WriteElement("Id", configuration.Id);
        WriteFilter(writer, configuration.Filter);
        writer.WriteEndElement("MetricsConfiguration");
    }

    public static S3MetricsConfiguration Parse(XmlReader reader)
    {
        string? id = null;
        S3Filter? filter = null;
        bool filterRead = false;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name == "Id" && id == null)
                id = reader.ReadString();
            else if (name == "Filter" && !filterRead)
            {
                filter = ParseFilter(reader);
                filterRead = true;
            }
        }

        return new S3MetricsConfiguration(id ?? string.Empty) { Filter = filter };
    }

    private static void WriteFilter(FastXmlWriter writer, S3Filter? filter)
    {
        if (filter == null)
            return;

        FilterHelper.Validate(filter, "Metrics", false);
        writer.WriteStartElement("Filter");

        if (filter.AndConditions != null)
        {
            writer.WriteStartElement("And");
            if (filter.AndConditions.Prefix != null)
                writer.WriteElement("Prefix", filter.AndConditions.Prefix);
            foreach (KeyValuePair<string, string> tag in filter.AndConditions.Tags)
                WriteTag(writer, tag);
            writer.WriteEndElement("And");
        }
        else if (filter.Prefix != null)
            writer.WriteElement("Prefix", filter.Prefix);
        else if (filter.Tag.HasValue)
            WriteTag(writer, filter.Tag.Value);

        writer.WriteEndElement("Filter");
    }

    private static void WriteTag(FastXmlWriter writer, KeyValuePair<string, string> tag)
    {
        writer.WriteStartElement("Tag");
        writer.WriteElement("Key", tag.Key);
        writer.WriteElement("Value", tag.Value);
        writer.WriteEndElement("Tag");
    }

    private static S3Filter ParseFilter(XmlReader reader)
    {
        S3AndCondition? and = null;
        string? prefix = null;
        KeyValuePair<string, string>? tag = null;
        bool andRead = false;
        bool tagRead = false;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name == "And" && !andRead)
            {
                and = ParseAnd(reader);
                andRead = true;
            }
            else if (name == "Prefix" && prefix == null)
                prefix = reader.ReadString();
            else if (name == "Tag" && !tagRead)
            {
                tag = ParseTag(reader);
                tagRead = true;
            }
        }

        return and != null ? new S3Filter { AndConditions = and } : new S3Filter { Prefix = prefix, Tag = tag };
    }

    private static S3AndCondition ParseAnd(XmlReader reader)
    {
        string? prefix = null;
        S3AndCondition condition = new S3AndCondition();

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name == "Prefix" && prefix == null) prefix = reader.ReadString();
            else if (name == "Tag") condition.Tags.Add(ParseTag(reader));
        }

        condition.Prefix = prefix;
        return condition;
    }

    private static KeyValuePair<string, string> ParseTag(XmlReader reader)
    {
        string? key = null;
        string? value = null;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name == "Key" && key == null) key = reader.ReadString();
            else if (name == "Value" && value == null) value = reader.ReadString();
        }

        return new KeyValuePair<string, string>(key ?? string.Empty, value ?? string.Empty);
    }
}