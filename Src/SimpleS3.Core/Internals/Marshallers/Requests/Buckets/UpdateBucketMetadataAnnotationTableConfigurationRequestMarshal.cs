using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class UpdateBucketMetadataAnnotationTableConfigurationRequestMarshal : IRequestMarshal<UpdateBucketMetadataAnnotationTableConfigurationRequest>
{
    public Stream MarshalRequest(UpdateBucketMetadataAnnotationTableConfigurationRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.MetadataAnnotationTable, string.Empty);

        S3AnnotationTableConfiguration configuration = request.Configuration;
        FastXmlWriter writer = new FastXmlWriter(512);
        writer.WriteStartElement("AnnotationTableConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
        writer.WriteElement("ConfigurationState", configuration.ConfigurationState.GetDisplayName());

        if (configuration.EncryptionConfiguration != null)
        {
            writer.WriteStartElement("EncryptionConfiguration");
            if (configuration.EncryptionConfiguration.KmsKeyArn != null)
                writer.WriteElement("KmsKeyArn", configuration.EncryptionConfiguration.KmsKeyArn);

            writer.WriteElement("SseAlgorithm", configuration.EncryptionConfiguration.SseAlgorithm.GetDisplayName());
            writer.WriteEndElement("EncryptionConfiguration");
        }

        if (configuration.Role != null)
            writer.WriteElement("Role", configuration.Role);

        writer.WriteEndElement("AnnotationTableConfiguration");
        return new MemoryStream(writer.GetBytes());
    }
}