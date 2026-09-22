using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

[SuppressMessage("Security", "S5332", Justification = "The HTTP URI is the namespace mandated by the S3 XML protocol, not a network endpoint.")]
internal static class InventoryConfigurationXml
{
    public static void Write(FastXmlWriter writer, S3InventoryConfiguration configuration)
    {
        writer.WriteStartElement("InventoryConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
        writer.WriteStartElement("Destination");
        writer.WriteStartElement("S3BucketDestination");
        S3InventoryDestination destination = configuration.Destination;
        if (destination.AccountId != null)
            writer.WriteElement("AccountId", destination.AccountId);
        writer.WriteElement("Bucket", destination.BucketArn);

        if (destination.Encryption != null)
        {
            writer.WriteStartElement("Encryption");

            if (destination.Encryption.Type == S3InventoryEncryptionType.SseS3)
                writer.WriteStartElement("SSE-S3");
            else
            {
                writer.WriteStartElement("SSE-KMS");
                if (destination.Encryption.KeyId != null)
                    writer.WriteElement("KeyId", destination.Encryption.KeyId);
            }

            writer.WriteEndElement(destination.Encryption.Type == S3InventoryEncryptionType.SseS3 ? "SSE-S3" : "SSE-KMS");
            writer.WriteEndElement("Encryption");
        }

        writer.WriteElement("Format", destination.Format);
        if (destination.Prefix != null)
            writer.WriteElement("Prefix", destination.Prefix);

        writer.WriteEndElement("S3BucketDestination");
        writer.WriteEndElement("Destination");
        writer.WriteElement("IsEnabled", configuration.Enabled.ToString().ToLowerInvariant());

        if (configuration.Prefix != null)
        {
            writer.WriteStartElement("Filter");
            writer.WriteElement("Prefix", configuration.Prefix);
            writer.WriteEndElement("Filter");
        }

        writer.WriteElement("Id", configuration.Id);
        writer.WriteElement("IncludedObjectVersions", configuration.IncludedObjectVersions);

        if (configuration.OptionalFields.Count > 0)
        {
            writer.WriteStartElement("OptionalFields");
            foreach (string field in configuration.OptionalFields)
                writer.WriteElement("Field", field);
            writer.WriteEndElement("OptionalFields");
        }

        writer.WriteStartElement("Schedule");
        writer.WriteElement("Frequency", configuration.ScheduleFrequency);
        writer.WriteEndElement("Schedule");
        writer.WriteEndElement("InventoryConfiguration");
    }

    public static S3InventoryConfiguration Parse(XmlReader reader)
    {
        string? id = null;
        string? enabled = null;
        string? prefix = null;
        string? versions = null;
        string? frequency = null;
        S3InventoryDestination? destination = null;
        List<string>? optionalFields = null;
        bool filterRead = false;
        bool scheduleRead = false;
        bool optionalFieldsRead = false;
        bool destinationRead = false;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            switch (name)
            {
                case "Id" when id == null:
                    id = reader.ReadString();
                    break;
                case "IsEnabled" when enabled == null:
                    enabled = reader.ReadString();
                    break;
                case "Filter" when !filterRead:
                    prefix = ParseFilter(reader);
                    filterRead = true;
                    break;
                case "IncludedObjectVersions" when versions == null:
                    versions = reader.ReadString();
                    break;
                case "Schedule" when !scheduleRead:
                    frequency = ParseSchedule(reader);
                    scheduleRead = true;
                    break;
                case "OptionalFields" when !optionalFieldsRead:
                    optionalFields = ParseOptionalFields(reader);
                    optionalFieldsRead = true;
                    break;
                case "Destination" when !destinationRead:
                    destination = ParseDestination(reader);
                    destinationRead = true;
                    break;
            }
        }

        S3InventoryConfiguration configuration = new S3InventoryConfiguration(id ?? string.Empty)
        {
            Enabled = enabled != null && XmlConvert.ToBoolean(enabled),
            Prefix = prefix
        };
        if (versions != null) configuration.IncludedObjectVersions = versions;
        if (frequency != null) configuration.ScheduleFrequency = frequency;
        if (destination != null) configuration.Destination = destination;

        if (optionalFields != null)
        {
            foreach (string field in optionalFields)
                configuration.OptionalFields.Add(field);
        }

        return configuration;
    }

    private static string? ParseFilter(XmlReader reader)
    {
        string? prefix = null;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name == "Prefix" && prefix == null)
                prefix = reader.ReadString();
        }

        return prefix;
    }

    private static string? ParseSchedule(XmlReader reader)
    {
        string? frequency = null;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name == "Frequency" && frequency == null)
                frequency = reader.ReadString();
        }

        return frequency;
    }

    private static List<string> ParseOptionalFields(XmlReader reader)
    {
        List<string> fields = [];

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name == "Field")
                fields.Add(reader.ReadString());
        }

        return fields;
    }

    private static S3InventoryDestination? ParseDestination(XmlReader reader)
    {
        S3InventoryDestination? destination = null;
        bool bucketDestinationRead = false;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name == "S3BucketDestination" && !bucketDestinationRead)
            {
                destination = ParseBucketDestination(reader);
                bucketDestinationRead = true;
            }
        }

        return destination;
    }

    private static S3InventoryDestination ParseBucketDestination(XmlReader reader)
    {
        string? accountId = null;
        string? bucket = null;
        string? format = null;
        string? prefix = null;
        S3InventoryEncryption? encryption = null;
        bool encryptionRead = false;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            switch (name)
            {
                case "AccountId" when accountId == null:
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
                case "Encryption" when !encryptionRead:
                    encryption = ParseEncryption(reader);
                    encryptionRead = true;
                    break;
            }
        }

        S3InventoryDestination destination = new S3InventoryDestination { AccountId = accountId, BucketArn = bucket!, Prefix = prefix, Encryption = encryption };
        if (format != null) destination.Format = format;
        return destination;
    }

    private static S3InventoryEncryption? ParseEncryption(XmlReader reader)
    {
        S3InventoryEncryption? encryption = null;
        bool encryptionRead = false;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name == "SSE-KMS" && !encryptionRead)
            {
                encryption = new S3InventoryEncryption { Type = S3InventoryEncryptionType.SseKms, KeyId = ParseKeyId(reader) };
                encryptionRead = true;
            }
            else if (name == "SSE-S3" && !encryptionRead)
            {
                encryption = new S3InventoryEncryption { Type = S3InventoryEncryptionType.SseS3 };
                encryptionRead = true;
            }
        }

        return encryption;
    }

    private static string? ParseKeyId(XmlReader reader)
    {
        string? keyId = null;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name == "KeyId" && keyId == null)
                keyId = reader.ReadString();
        }

        return keyId;
    }
}