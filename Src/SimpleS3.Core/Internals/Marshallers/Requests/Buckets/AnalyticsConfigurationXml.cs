using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

[SuppressMessage("Security", "S5332", Justification = "The HTTP URI is the namespace mandated by the S3 XML protocol, not a network endpoint.")]
internal static class AnalyticsConfigurationXml
{
    public static void Write(FastXmlWriter writer, S3AnalyticsConfiguration configuration)
    {
        writer.WriteStartElement("AnalyticsConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
        writer.WriteElement("Id", configuration.Id);
        WriteFilter(writer, configuration.Filter);
        writer.WriteStartElement("StorageClassAnalysis");

        if (configuration.DataExport != null)
        {
            S3AnalyticsDataExport export = configuration.DataExport;
            writer.WriteStartElement("DataExport");
            writer.WriteStartElement("Destination");
            writer.WriteStartElement("S3BucketDestination");
            writer.WriteElement("Bucket", export.BucketArn);
            if (export.BucketAccountId != null)
                writer.WriteElement("BucketAccountId", export.BucketAccountId);
            writer.WriteElement("Format", export.Format);
            if (export.Prefix != null)
                writer.WriteElement("Prefix", export.Prefix);
            writer.WriteEndElement("S3BucketDestination");
            writer.WriteEndElement("Destination");
            writer.WriteElement("OutputSchemaVersion", export.OutputSchemaVersion);
            writer.WriteEndElement("DataExport");
        }

        writer.WriteEndElement("StorageClassAnalysis");
        writer.WriteEndElement("AnalyticsConfiguration");
    }

    public static S3AnalyticsConfiguration Parse(XmlReader reader)
    {
        string? id = null;
        S3Filter? filter = null;
        S3AnalyticsDataExport? dataExport = null;
        bool filterRead = false;
        bool analysisRead = false;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            switch (name)
            {
                case "Id" when id == null:
                    id = reader.ReadString();
                    break;
                case "Filter" when !filterRead:
                    filter = ParseFilter(reader);
                    filterRead = true;
                    break;
                case "StorageClassAnalysis" when !analysisRead:
                    dataExport = ParseDataExport(reader);
                    analysisRead = true;
                    break;
            }
        }

        S3AnalyticsConfiguration configuration = new S3AnalyticsConfiguration(id ?? string.Empty) { Filter = filter, DataExport = dataExport };
        return configuration;
    }

    private static void WriteFilter(FastXmlWriter writer, S3Filter? filter)
    {
        if (filter == null)
            return;

        FilterHelper.Validate(filter, "Analytics", false);
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

    private static S3AnalyticsDataExport? ParseDataExport(XmlReader reader)
    {
        S3AnalyticsDataExport? export = null;
        bool exportRead = false;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name == "DataExport" && !exportRead)
            {
                export = ParseExport(reader);
                exportRead = true;
            }
        }

        return export;
    }

    private static S3AnalyticsDataExport? ParseExport(XmlReader reader)
    {
        string? schema = null;
        S3AnalyticsDataExport? export = null;
        bool destinationRead = false;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            switch (name)
            {
                case "Destination" when !destinationRead:
                    export = ParseDestination(reader);
                    destinationRead = true;
                    break;
                case "OutputSchemaVersion" when schema == null:
                    schema = reader.ReadString();
                    break;
            }
        }

        if (export != null && schema != null)
            export.OutputSchemaVersion = schema;
        return export;
    }

    private static S3AnalyticsDataExport? ParseDestination(XmlReader reader)
    {
        S3AnalyticsDataExport? export = null;
        bool bucketDestinationRead = false;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name == "S3BucketDestination" && !bucketDestinationRead)
            {
                export = ParseBucketDestination(reader);
                bucketDestinationRead = true;
            }
        }

        return export;
    }

    private static S3AnalyticsDataExport ParseBucketDestination(XmlReader reader)
    {
        string? accountId = null;
        string? bucket = null;
        string? format = null;
        string? prefix = null;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            switch (name)
            {
                case "BucketAccountId" when accountId == null:
                    accountId = reader.ReadString();
                    break;
                case "Bucket" when bucket == null:
                    bucket = reader.ReadString();
                    break;
                case "Format" when format == null:
                    format = reader.ReadString();
                    break;
                case "Prefix" when prefix == null:
                    prefix = reader.ReadString();
                    break;
            }
        }

        S3AnalyticsDataExport export = new S3AnalyticsDataExport { BucketAccountId = accountId, BucketArn = bucket!, Prefix = prefix };
        if (format != null) export.Format = format;
        return export;
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
            switch (name)
            {
                case "And" when !andRead:
                    and = ParseAnd(reader);
                    andRead = true;
                    break;
                case "Prefix" when prefix == null:
                    prefix = reader.ReadString();
                    break;
                case "Tag" when !tagRead:
                    tag = ParseTag(reader);
                    tagRead = true;
                    break;
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
            if (name == "Prefix" && prefix == null)
                prefix = reader.ReadString();
            else if (name == "Tag")
                condition.Tags.Add(ParseTag(reader));
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