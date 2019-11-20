using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request.Object
{
    [UsedImplicitly]
    internal class RestoreObjectRequestMarshal : IRequestMarshal<RestoreObjectRequest>
    {
        public Stream MarshalRequest(RestoreObjectRequest request, IS3Config config)
        {
            request.AddQueryParameter("restore", string.Empty);
            request.AddQueryParameter(AmzHeaders.XAmzVersionId, request.VersionId);
            request.AddHeader(AmzHeaders.XAmzRequestPayer, request.RequestPayer == Payer.Requester ? "requester" : null);

            StringBuilder sb = new StringBuilder(512);
            sb.Append("<RestoreRequest xmlns=\"http://s3.amazonaws.com/doc/2006-03-01/\">");

            if (request.Days > 0)
                sb.Append("<Days>").Append(request.Days).Append("</Days>");

            if (request.RequestType != RestoreRequestType.Unknown)
                sb.Append("<Type>").Append(ValueHelper.EnumToString(request.RequestType)).Append("</Type>");

            if (request.RequestTier != RetrievalTier.Unknown)
                sb.Append("<Tier>").Append(ValueHelper.EnumToString(request.RequestTier)).Append("</Tier>");

            if (request.GlacierTier != RetrievalTier.Unknown)
                sb.Append("<GlacierJobParameters><Tier>").Append(ValueHelper.EnumToString(request.GlacierTier)).Append("</Tier></GlacierJobParameters>");

            if (request.Description != null)
                sb.Append("<Description>").Append(request.Description).Append("</Description>");

            if (request.SelectParameters.InputFormat != null || request.SelectParameters.OutputFormat != null || request.SelectParameters.Expression != null || request.SelectParameters.ExpressionType != ExpressionType.Unknown)
            {
                sb.Append("<SelectParameters>");

                if (request.SelectParameters.InputFormat != null)
                {
                    sb.Append("<InputSerialization>");

                    if (request.SelectParameters.InputFormat.CompressionType != CompressionType.Unknown)
                        sb.Append("<CompressionType>").Append(ValueHelper.EnumToString(request.SelectParameters.InputFormat.CompressionType)).Append("</CompressionType>");

                    switch (request.SelectParameters.InputFormat)
                    {
                        case S3CsvInputFormat csvInput:
                            {
                                sb.Append("<CSV>");

                                if (csvInput.HeaderUsage != HeaderUsage.Unknown)
                                    sb.Append("<FileHeaderInfo>").Append(ValueHelper.EnumToString(csvInput.HeaderUsage)).Append("</FileHeaderInfo>");

                                if (csvInput.CommentCharacter != null)
                                    sb.Append("<Comments>").Append(ConvertChar(csvInput.CommentCharacter)).Append("</Comments>");

                                if (csvInput.QuoteEscapeCharacter != null)
                                    sb.Append("<QuoteEscapeCharacter>").Append(ConvertChar(csvInput.QuoteEscapeCharacter)).Append("</QuoteEscapeCharacter>");

                                if (csvInput.RecordDelimiter != null)
                                    sb.Append("<RecordDelimiter>").Append(ConvertChar(csvInput.RecordDelimiter)).Append("</RecordDelimiter>");

                                if (csvInput.FieldDelimiter != null)
                                    sb.Append("<FieldDelimiter>").Append(ConvertChar(csvInput.FieldDelimiter)).Append("</FieldDelimiter>");

                                if (csvInput.QuoteCharacter != null)
                                    sb.Append("<QuoteCharacter>").Append(ConvertChar(csvInput.QuoteCharacter)).Append("</QuoteCharacter>");

                                if (csvInput.AllowQuotedRecordDelimiter != null)
                                    sb.Append("<AllowQuotedRecordDelimiter>").Append(csvInput.AllowQuotedRecordDelimiter).Append("</AllowQuotedRecordDelimiter>");

                                sb.Append("</CSV>");

                                break;
                            }
                        case S3JsonInputFormat jsonInput:
                            {
                                sb.Append("<JSON>");

                                if (jsonInput.JsonType != JsonType.Unknown)
                                    sb.Append("<Type>").Append(ValueHelper.EnumToString(jsonInput.JsonType)).Append("</Type>");

                                sb.Append("</JSON>");

                                break;
                            }
                        case S3ParquetInputFormat _:
                            sb.Append("<Parquet></Parquet>");
                            break;
                    }

                    sb.Append("</InputSerialization>");
                }

                if (request.SelectParameters.ExpressionType != ExpressionType.Unknown)
                    sb.Append("<ExpressionType>").Append(ValueHelper.EnumToString(request.SelectParameters.ExpressionType)).Append("</ExpressionType>");

                if (request.SelectParameters.Expression != null)
                    sb.Append("<Expression>").Append(request.SelectParameters.Expression).Append("</Expression>");

                if (request.SelectParameters.OutputFormat != null)
                {
                    sb.Append("<OutputSerialization>");

                    switch (request.SelectParameters.OutputFormat)
                    {
                        case S3CsvOutputFormat csvOutput:
                            {
                                sb.Append("<CSV>");

                                if (csvOutput.FieldDelimiter != null)
                                    sb.Append("<FieldDelimiter>").Append(ConvertChar(csvOutput.FieldDelimiter)).Append("</FieldDelimiter>");

                                if (csvOutput.QuoteCharacter != null)
                                    sb.Append("<QuoteCharacter>").Append(ConvertChar(csvOutput.QuoteCharacter)).Append("</QuoteCharacter>");

                                if (csvOutput.QuoteEscapeCharacter != null)
                                    sb.Append("<QuoteEscapeCharacter>").Append(ConvertChar(csvOutput.QuoteEscapeCharacter)).Append("</QuoteEscapeCharacter>");

                                if (csvOutput.QuoteFields != QuoteFields.Unknown)
                                    sb.Append("<QuoteFields>").Append(ValueHelper.EnumToString(csvOutput.QuoteFields)).Append("</QuoteFields>");

                                if (csvOutput.RecordDelimiter != null)
                                    sb.Append("<RecordDelimiter>").Append(ConvertChar(csvOutput.RecordDelimiter)).Append("</RecordDelimiter>");

                                sb.Append("</CSV>");

                                break;
                            }
                        case S3JsonOutputFormat jsonOutput:
                            {
                                if (jsonOutput.RecordDelimiter != null)
                                    sb.Append("<RecordDelimiter>").Append(jsonOutput.RecordDelimiter).Append("</RecordDelimiter>");

                                break;
                            }
                    }

                    sb.Append("</OutputSerialization>");
                }

                sb.Append("</SelectParameters>");
            }

            if (request.OutputLocation != null)
            {
                sb.Append("<OutputLocation>");

                if (request.OutputLocation is S3OutputLocation s3Out)
                {
                    sb.Append("<S3>");

                    //These two are required, so we don't check for null
                    sb.Append("<BucketName>").Append(s3Out.BucketName).Append("</BucketName>");
                    sb.Append("<Prefix>").Append(s3Out.Prefix).Append("</Prefix>");

                    if (s3Out.StorageClass != StorageClass.Unknown)
                        sb.Append("<StorageClass>").Append(ValueHelper.EnumToString(s3Out.StorageClass)).Append("</StorageClass>");

                    if (s3Out.Acl != ObjectCannedAcl.Unknown)
                        sb.Append("<CannedACL>").Append(ValueHelper.EnumToString(s3Out.Acl)).Append("</CannedACL>");

                    //TODO: AccessControlList support

                    if (s3Out.SseAlgorithm != SseAlgorithm.Unknown)
                    {
                        sb.Append("<Encryption>");
                        sb.Append("<EncryptionType>").Append(ValueHelper.EnumToString(s3Out.SseAlgorithm)).Append("<EncryptionType>");

                        string context = s3Out.SseContext.Build();
                        if (context != null)
                            sb.Append("<KMSContext>").Append(context).Append("<KMSContext>");

                        if (s3Out.SseKmsKeyId != null)
                            sb.Append("<KMSKeyId>").Append(s3Out.SseKmsKeyId).Append("<KMSKeyId>");

                        sb.Append("</Encryption>");
                    }

                    List<KeyValuePair<string, string>> tags = s3Out.Tags.ToList();

                    if (tags.Count > 0)
                    {
                        sb.Append("<Tagging><TagSet>");

                        foreach (KeyValuePair<string, string> tag in tags)
                        {
                            sb.Append("<Tag>");
                            sb.Append("<Key>").Append(tag.Key).Append("</Key>");
                            sb.Append("<Value>").Append(tag.Value).Append("</Value>");
                            sb.Append("</Tag>");
                        }

                        sb.Append("</TagSet></Tagging>");
                    }

                    List<KeyValuePair<string, string>> metadata = s3Out.Metadata.ToList();

                    if (metadata.Count > 0)
                    {
                        sb.Append("<UserMetadata>");

                        foreach (KeyValuePair<string, string> meta in metadata)
                        {
                            sb.Append("<MetadataEntry>");
                            sb.Append("<Name>").Append(meta.Key).Append("</Name>");
                            sb.Append("<Value>").Append(meta.Value).Append("</Value>");
                            sb.Append("</MetadataEntry>");
                        }

                        sb.Append("</UserMetadata>");
                    }

                    sb.Append("</S3>");
                }

                sb.Append("</OutputLocation>");
            }

            sb.Append("</RestoreRequest>");

            return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
        }

        //Note: This might not be needed. There is lacking documentation on RestoreObject with select, and I can't test it since it is down 99% of the time.
        private static string ConvertChar(char? c)
        {
            if (c == '\n')
                return "\\n";

            if (c == '\t')
                return "\\t";

            return c.ToString();
        }
    }
}