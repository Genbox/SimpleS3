using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects;

internal sealed class PutObjectRetentionRequestMarshal : IRequestMarshal<PutObjectRetentionRequest>
{
    public Stream MarshalRequest(PutObjectRetentionRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Retention, string.Empty);

        FastXmlWriter writer = new FastXmlWriter(256);
        writer.WriteStartElement("Retention", XmlConstants.S3Namespace);

        if (request.LockMode != LockMode.Unknown)
            writer.WriteElement("Mode", request.LockMode.GetDisplayName());

        if (request.LockRetainUntil.HasValue)
            writer.WriteElement("RetainUntilDate", ValueHelper.DateToString(request.LockRetainUntil.Value, DateTimeFormat.Iso8601DateTimeExt));

        writer.WriteEndElement("Retention");
        return new MemoryStream(writer.GetBytes());
    }
}