using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;

internal sealed class GetObjectRetentionResponseMarshal : IResponseMarshal<GetObjectRetentionResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetObjectRetentionResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

        using XmlReader reader = XmlHelper.CreateReader(responseStream);

        while (!reader.EOF)
        {
            if (!reader.IsStartElement())
            {
                reader.Read();
                continue;
            }

            switch (reader.Name)
            {
                case "Mode":
                    response.LockMode = Core.Enums.Enums.LockMode.Parse(reader.ReadElementContentAsString(), LockModeFormat.DisplayName);
                    break;
                case "RetainUntilDate":
                    response.LockRetainUntil = ValueHelper.ParseDate(reader.ReadElementContentAsString(), DateTimeFormat.Iso8601DateTimeExt);
                    break;
                default:
                    reader.Read();
                    break;
            }
        }
    }
}