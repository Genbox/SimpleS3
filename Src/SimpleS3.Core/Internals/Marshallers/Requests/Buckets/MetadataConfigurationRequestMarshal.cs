using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal static class MetadataConfigurationXml
{
    internal static void WriteEncryption(FastXmlWriter writer, S3MetadataTableEncryptionConfiguration? encryption)
    {
        if (encryption == null) return;
        writer.WriteStartElement("EncryptionConfiguration");
        if (encryption.KmsKeyArn != null) writer.WriteElement("KmsKeyArn", encryption.KmsKeyArn);
        writer.WriteElement("SseAlgorithm", encryption.SseAlgorithm.GetDisplayName());
        writer.WriteEndElement("EncryptionConfiguration");
    }

    internal static void WriteExpiration(FastXmlWriter writer, S3RecordExpiration expiration)
    {
        writer.WriteStartElement("RecordExpiration");
        if (expiration.Days.HasValue) writer.WriteElement("Days", expiration.Days.Value.ToString(CultureInfo.InvariantCulture));
        writer.WriteElement("Expiration", expiration.Expiration.GetDisplayName());
        writer.WriteEndElement("RecordExpiration");
    }

    internal static void WriteInventory(FastXmlWriter writer, S3InventoryTableConfiguration configuration)
    {
        writer.WriteStartElement("InventoryTableConfiguration");
        writer.WriteElement("ConfigurationState", configuration.ConfigurationState.GetDisplayName());
        WriteEncryption(writer, configuration.EncryptionConfiguration);
        writer.WriteEndElement("InventoryTableConfiguration");
    }

    internal static void WriteJournal(FastXmlWriter writer, S3JournalTableConfiguration configuration)
    {
        writer.WriteStartElement("JournalTableConfiguration");
        WriteEncryption(writer, configuration.EncryptionConfiguration);
        WriteExpiration(writer, configuration.RecordExpiration);
        writer.WriteEndElement("JournalTableConfiguration");
    }

    internal static void WriteAnnotation(FastXmlWriter writer, S3AnnotationTableConfiguration configuration)
    {
        writer.WriteStartElement("AnnotationTableConfiguration");
        writer.WriteElement("ConfigurationState", configuration.ConfigurationState.GetDisplayName());
        WriteEncryption(writer, configuration.EncryptionConfiguration);
        if (configuration.Role != null) writer.WriteElement("Role", configuration.Role);
        writer.WriteEndElement("AnnotationTableConfiguration");
    }
}

[SuppressMessage("Security", "S5332", Justification = "The HTTP URI is the namespace mandated by the S3 XML protocol, not a network endpoint.")]
internal sealed class CreateBucketMetadataConfigurationRequestMarshal : IRequestMarshal<CreateBucketMetadataConfigurationRequest>
{
    public Stream MarshalRequest(CreateBucketMetadataConfigurationRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter("metadataConfiguration", string.Empty);
        FastXmlWriter writer = new FastXmlWriter(512);
        writer.WriteStartElement("MetadataConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
        MetadataConfigurationXml.WriteJournal(writer, request.Configuration.JournalTableConfiguration);
        if (request.Configuration.InventoryTableConfiguration != null) MetadataConfigurationXml.WriteInventory(writer, request.Configuration.InventoryTableConfiguration);
        if (request.Configuration.AnnotationTableConfiguration != null) MetadataConfigurationXml.WriteAnnotation(writer, request.Configuration.AnnotationTableConfiguration);
        writer.WriteEndElement("MetadataConfiguration");
        return new MemoryStream(writer.GetBytes());
    }
}

[SuppressMessage("Security", "S5332", Justification = "The HTTP URI is the namespace mandated by the S3 XML protocol, not a network endpoint.")]
internal sealed class UpdateBucketMetadataInventoryTableConfigurationRequestMarshal : IRequestMarshal<UpdateBucketMetadataInventoryTableConfigurationRequest>
{
    public Stream MarshalRequest(UpdateBucketMetadataInventoryTableConfigurationRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter("metadataInventoryTable", string.Empty);
        FastXmlWriter writer = new FastXmlWriter(256);
        writer.WriteStartElement("InventoryTableConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
        writer.WriteElement("ConfigurationState", request.Configuration.ConfigurationState.GetDisplayName());
        MetadataConfigurationXml.WriteEncryption(writer, request.Configuration.EncryptionConfiguration);
        writer.WriteEndElement("InventoryTableConfiguration");
        return new MemoryStream(writer.GetBytes());
    }
}

[SuppressMessage("Security", "S5332", Justification = "The HTTP URI is the namespace mandated by the S3 XML protocol, not a network endpoint.")]
internal sealed class UpdateBucketMetadataJournalTableConfigurationRequestMarshal : IRequestMarshal<UpdateBucketMetadataJournalTableConfigurationRequest>
{
    public Stream MarshalRequest(UpdateBucketMetadataJournalTableConfigurationRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter("metadataJournalTable", string.Empty);
        FastXmlWriter writer = new FastXmlWriter(256);
        writer.WriteStartElement("JournalTableConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
        MetadataConfigurationXml.WriteExpiration(writer, request.Configuration.RecordExpiration);
        writer.WriteEndElement("JournalTableConfiguration");
        return new MemoryStream(writer.GetBytes());
    }
}

internal sealed class DeleteBucketMetadataConfigurationRequestMarshal : IRequestMarshal<DeleteBucketMetadataConfigurationRequest>
{
    public Stream? MarshalRequest(DeleteBucketMetadataConfigurationRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter("metadataConfiguration", string.Empty);
        return null;
    }
}

internal sealed class GetBucketMetadataConfigurationRequestMarshal : IRequestMarshal<GetBucketMetadataConfigurationRequest>
{
    public Stream? MarshalRequest(GetBucketMetadataConfigurationRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter("metadataConfiguration", string.Empty);
        return null;
    }
}