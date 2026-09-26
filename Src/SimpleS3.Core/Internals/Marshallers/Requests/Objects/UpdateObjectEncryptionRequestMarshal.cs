using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects;

internal sealed class UpdateObjectEncryptionRequestMarshal : IRequestMarshal<UpdateObjectEncryptionRequest>
{
    public Stream MarshalRequest(UpdateObjectEncryptionRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Encryption, string.Empty);
        FastXmlWriter writer = new FastXmlWriter(256);
        writer.WriteStartElement("ObjectEncryption", "http://s3.amazonaws.com/doc/2006-03-01/");
        writer.WriteStartElement("SSE-KMS");
        if (request.BucketKeyEnabled.HasValue)
            writer.WriteElement("BucketKeyEnabled", request.BucketKeyEnabled.Value ? "true" : "false");
        writer.WriteElement("KMSKeyArn", request.KmsKeyArn);
        writer.WriteEndElement("SSE-KMS");
        writer.WriteEndElement("ObjectEncryption");
        return new MemoryStream(writer.GetBytes());
    }
}