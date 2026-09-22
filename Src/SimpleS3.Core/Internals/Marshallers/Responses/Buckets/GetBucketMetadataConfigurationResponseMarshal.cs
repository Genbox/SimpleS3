using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class GetBucketMetadataConfigurationResponseMarshal : IResponseMarshal<GetBucketMetadataConfigurationResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetBucketMetadataConfigurationResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        using XmlReader reader = XmlHelper.CreateReader(responseStream);
        reader.MoveToContent();

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name != "MetadataConfigurationResult")
                continue;

            S3MetadataConfigurationResult value = new S3MetadataConfigurationResult();
            ReadConfiguration(value, reader);
            response.Configuration = value;
            return;
        }
    }

    private static void ReadConfiguration(S3MetadataConfigurationResult configuration, XmlReader reader)
    {
        bool destinationRead = false;
        bool journalRead = false;
        bool inventoryRead = false;
        bool annotationRead = false;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            switch (name)
            {
                case "DestinationResult" when !destinationRead:
                    configuration.Destination = ReadDestination(reader);
                    destinationRead = true;
                    break;
                case "JournalTableConfigurationResult" when !journalRead:
                    configuration.JournalTable = ReadJournal(reader);
                    journalRead = true;
                    break;
                case "InventoryTableConfigurationResult" when !inventoryRead:
                    configuration.InventoryTable = ReadInventory(reader);
                    inventoryRead = true;
                    break;
                case "AnnotationTableConfigurationResult" when !annotationRead:
                    configuration.AnnotationTable = ReadAnnotation(reader);
                    annotationRead = true;
                    break;
            }
        }
    }

    private static S3MetadataDestinationResult ReadDestination(XmlReader reader)
    {
        S3MetadataDestinationResult result = new S3MetadataDestinationResult();
        foreach (string name in XmlHelper.ReadChildren(reader))
            SetDestinationValue(result, name, reader);
        return result;
    }

    private static S3MetadataJournalTableResult ReadJournal(XmlReader reader)
    {
        S3MetadataJournalTableResult result = new S3MetadataJournalTableResult();
        bool expirationRead = false;
        bool errorRead = false;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            switch (name)
            {
                case "Error" when !errorRead:
                    result.Error = ReadError(reader);
                    errorRead = true;
                    break;
                case "RecordExpiration" when !expirationRead:
                    ReadExpiration(result, reader);
                    expirationRead = true;
                    break;
                case "TableArn" when result.TableArn == null:
                    result.TableArn = reader.ReadString();
                    break;
                case "TableName" when result.TableName == null:
                    result.TableName = reader.ReadString();
                    break;
                case "TableStatus" when result.TableStatus == null:
                    result.TableStatus = reader.ReadString();
                    break;
            }
        }

        return result;
    }

    private static S3MetadataInventoryTableResult ReadInventory(XmlReader reader)
    {
        S3MetadataInventoryTableResult result = new S3MetadataInventoryTableResult();
        bool errorRead = false;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            switch (name)
            {
                case "ConfigurationState" when result.ConfigurationState == null:
                    result.ConfigurationState = reader.ReadString();
                    break;
                case "Error" when !errorRead:
                    result.Error = ReadError(reader);
                    errorRead = true;
                    break;
                case "TableArn" when result.TableArn == null:
                    result.TableArn = reader.ReadString();
                    break;
                case "TableName" when result.TableName == null:
                    result.TableName = reader.ReadString();
                    break;
                case "TableStatus" when result.TableStatus == null:
                    result.TableStatus = reader.ReadString();
                    break;
            }
        }

        return result;
    }

    private static S3MetadataAnnotationTableResult ReadAnnotation(XmlReader reader)
    {
        S3MetadataAnnotationTableResult result = new S3MetadataAnnotationTableResult();
        bool errorRead = false;

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            switch (name)
            {
                case "ConfigurationState" when result.ConfigurationState == null:
                    result.ConfigurationState = reader.ReadString();
                    break;
                case "Error" when !errorRead:
                    result.Error = ReadError(reader);
                    errorRead = true;
                    break;
                case "Role" when result.Role == null:
                    result.Role = reader.ReadString();
                    break;
                case "TableArn" when result.TableArn == null:
                    result.TableArn = reader.ReadString();
                    break;
                case "TableName" when result.TableName == null:
                    result.TableName = reader.ReadString();
                    break;
                case "TableStatus" when result.TableStatus == null:
                    result.TableStatus = reader.ReadString();
                    break;
            }
        }

        return result;
    }

    private static S3MetadataError ReadError(XmlReader reader)
    {
        S3MetadataError result = new S3MetadataError();

        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name == "ErrorCode" && result.ErrorCode == null) result.ErrorCode = reader.ReadString();
            if (name == "ErrorMessage" && result.ErrorMessage == null) result.ErrorMessage = reader.ReadString();
        }

        return result;
    }

    private static void SetDestinationValue(S3MetadataDestinationResult result, string name, XmlReader reader)
    {
        if (name == "TableBucketArn" && result.TableBucketArn == null) result.TableBucketArn = reader.ReadString();
        if (name == "TableBucketType" && result.TableBucketType == null) result.TableBucketType = reader.ReadString();
        if (name == "TableNamespace" && result.TableNamespace == null) result.TableNamespace = reader.ReadString();
    }

    private static void ReadExpiration(S3MetadataJournalTableResult result, XmlReader reader)
    {
        foreach (string name in XmlHelper.ReadChildren(reader))
        {
            if (name == "Expiration" && result.Expiration == null) result.Expiration = reader.ReadString();
            if (name == "Days" && result.ExpirationDays == null) result.ExpirationDays = ValueHelper.ParseInt(reader.ReadString());
        }
    }
}