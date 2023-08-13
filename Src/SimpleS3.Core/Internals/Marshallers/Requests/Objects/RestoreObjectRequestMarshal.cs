using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects;

internal class RestoreObjectRequestMarshal : IRequestMarshal<RestoreObjectRequest>
{
    public Stream MarshalRequest(RestoreObjectRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Restore, string.Empty);

        FastXmlWriter xml = new FastXmlWriter(512);
        xml.WriteStartElement("RestoreRequest", "http://s3.amazonaws.com/doc/2006-03-01/");

        if (request.Days > 0)
            xml.WriteElement("Days", request.Days);

        if (request.RequestType != RestoreRequestType.Unknown)
            xml.WriteElement("Type", ValueHelper.EnumToString(request.RequestType));

        if (request.RequestTier != RetrievalTier.Unknown)
            xml.WriteElement("Tier", ValueHelper.EnumToString(request.RequestTier));

        if (request.GlacierTier != RetrievalTier.Unknown)
        {
            xml.WriteStartElement("GlacierJobParameters");
            xml.WriteElement("Tier", ValueHelper.EnumToString(request.GlacierTier));
            xml.WriteEndElement("GlacierJobParameters");
        }

        if (request.Description != null)
            xml.WriteElement("Description", request.Description);

        if (request.SelectParameters != null)
        {
            xml.WriteStartElement("SelectParameters");

            if (request.SelectParameters.InputFormat != null)
            {
                xml.WriteStartElement("InputSerialization");

                if (request.SelectParameters.InputFormat.CompressionType != CompressionType.Unknown)
                    xml.WriteElement("CompressionType", ValueHelper.EnumToString(request.SelectParameters.InputFormat.CompressionType));

                switch (request.SelectParameters.InputFormat)
                {
                    case S3CsvInputFormat csvInput:
                    {
                        xml.WriteStartElement("CSV");

                        if (csvInput.HeaderUsage != HeaderUsage.Unknown)
                            xml.WriteElement("FileHeaderInfo", ValueHelper.EnumToString(csvInput.HeaderUsage));

                        if (csvInput.CommentCharacter != null)
                            xml.WriteElement("Comments", ConvertChar(csvInput.CommentCharacter));

                        if (csvInput.QuoteEscapeCharacter != null)
                            xml.WriteElement("QuoteEscapeCharacter", ConvertChar(csvInput.QuoteEscapeCharacter));

                        if (csvInput.RecordDelimiter != null)
                            xml.WriteElement("RecordDelimiter", ConvertChar(csvInput.RecordDelimiter));

                        if (csvInput.FieldDelimiter != null)
                            xml.WriteElement("FieldDelimiter", ConvertChar(csvInput.FieldDelimiter));

                        if (csvInput.QuoteCharacter != null)
                            xml.WriteElement("QuoteCharacter", ConvertChar(csvInput.QuoteCharacter));

                        if (csvInput.AllowQuotedRecordDelimiter != null)
                            xml.WriteElement("AllowQuotedRecordDelimiter", csvInput.AllowQuotedRecordDelimiter);

                        xml.WriteEndElement("CSV");

                        break;
                    }
                    case S3JsonInputFormat jsonInput:
                    {
                        xml.WriteStartElement("JSON");

                        if (jsonInput.JsonType != JsonType.Unknown)
                            xml.WriteElement("Type", ValueHelper.EnumToString(jsonInput.JsonType));

                        xml.WriteEndElement("JSON");

                        break;
                    }
                    case S3ParquetInputFormat _:
                        xml.WriteElement("Parquet", string.Empty);
                        break;
                }

                xml.WriteEndElement("InputSerialization");
            }

            if (request.SelectParameters.ExpressionType != ExpressionType.Unknown)
                xml.WriteElement("ExpressionType", ValueHelper.EnumToString(request.SelectParameters.ExpressionType));

            xml.WriteElement("Expression", request.SelectParameters.Expression);

            xml.WriteStartElement("OutputSerialization");

            switch (request.SelectParameters.OutputFormat)
            {
                case S3CsvOutputFormat csvOutput:
                    xml.WriteStartElement("CSV");

                    if (csvOutput.FieldDelimiter != null)
                        xml.WriteElement("FieldDelimiter", ConvertChar(csvOutput.FieldDelimiter));

                    if (csvOutput.QuoteCharacter != null)
                        xml.WriteElement("QuoteCharacter", ConvertChar(csvOutput.QuoteCharacter));

                    if (csvOutput.QuoteEscapeCharacter != null)
                        xml.WriteElement("QuoteEscapeCharacter", ConvertChar(csvOutput.QuoteEscapeCharacter));

                    if (csvOutput.QuoteFields != QuoteField.Unknown)
                        xml.WriteElement("QuoteFields", ValueHelper.EnumToString(csvOutput.QuoteFields));

                    if (csvOutput.RecordDelimiter != null)
                        xml.WriteElement("RecordDelimiter", ConvertChar(csvOutput.RecordDelimiter));

                    xml.WriteEndElement("CSV");

                    break;
                case S3JsonOutputFormat jsonOutput:
                    xml.WriteElement("RecordDelimiter", jsonOutput.RecordDelimiter);

                    break;
            }

            xml.WriteEndElement("OutputSerialization");

            xml.WriteEndElement("SelectParameters");
        }

        if (request.OutputLocation != null)
        {
            xml.WriteStartElement("OutputLocation");
            xml.WriteStartElement("S3");

            //These two are required, so we don't check for null
            xml.WriteElement("BucketName", request.OutputLocation.BucketName);

            if (request.OutputLocation.Prefix != null)
                xml.WriteElement("Prefix", request.OutputLocation.Prefix);

            if (request.OutputLocation.StorageClass != StorageClass.Unknown)
                xml.WriteElement("StorageClass", ValueHelper.EnumToString(request.OutputLocation.StorageClass));

            if (request.OutputLocation.Acl != ObjectCannedAcl.Unknown)
                xml.WriteElement("CannedACL", ValueHelper.EnumToString(request.OutputLocation.Acl));

            //TODO: AccessControlList support

            if (request.OutputLocation.SseAlgorithm != SseAlgorithm.Unknown)
            {
                xml.WriteStartElement("Encryption");
                xml.WriteElement("EncryptionType", ValueHelper.EnumToString(request.OutputLocation.SseAlgorithm));

                string? context = request.OutputLocation.SseContext.Build();
                if (context != null)
                    xml.WriteElement("KMSContext", context);

                if (request.OutputLocation.SseKmsKeyId != null)
                    xml.WriteElement("KMSKeyId", request.OutputLocation.SseKmsKeyId);

                xml.WriteEndElement("Encryption");
            }

            KeyValuePair<string, string>[] tags = request.OutputLocation.Tags.ToArray();

            if (tags.Length > 0)
            {
                xml.WriteStartElement("Tagging");
                xml.WriteStartElement("TagSet");

                foreach (KeyValuePair<string, string> tag in tags)
                {
                    xml.WriteStartElement("Tag");
                    xml.WriteElement("Key", tag.Key);
                    xml.WriteElement("Value", tag.Value);
                    xml.WriteEndElement("Tag");
                }

                xml.WriteEndElement("TagSet");
                xml.WriteEndElement("Tagging");
            }

            KeyValuePair<string,string>[] metadata = request.OutputLocation.Metadata.ToArray();

            if (metadata.Length > 0)
            {
                xml.WriteStartElement("UserMetadata");

                foreach (KeyValuePair<string, string> meta in metadata)
                {
                    xml.WriteStartElement("MetadataEntry");
                    xml.WriteElement("Name", meta.Key);
                    xml.WriteElement("Value", meta.Value);
                    xml.WriteEndElement("MetadataEntry");
                }

                xml.WriteEndElement("UserMetadata");
            }

            xml.WriteEndElement("S3");

            xml.WriteEndElement("OutputLocation");
        }

        xml.WriteEndElement("RestoreRequest");

        return new MemoryStream(xml.GetBytes());
    }

    //Note: This might not be needed. There is lacking documentation on RestoreObject with select, and I can't test it since it is down 99% of the time.
    private static string ConvertChar(char? c)
    {
        if (c == '\n')
            return "\\n";

        if (c == '\t')
            return "\\t";

        return c.ToString()!;
    }
}