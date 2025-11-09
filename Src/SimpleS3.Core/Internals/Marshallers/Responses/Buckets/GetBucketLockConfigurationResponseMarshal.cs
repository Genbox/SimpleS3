using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class GetBucketLockConfigurationResponseMarshal : IResponseMarshal<GetBucketLockConfigurationResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetBucketLockConfigurationResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        using XmlTextReader xmlReader = new XmlTextReader(responseStream);
        xmlReader.ReadToDescendant("DefaultRetention");

        foreach (string name in XmlHelper.ReadElements(xmlReader))
        {
            switch (name)
            {
                case "Mode":
                    response.LockMode = Core.Enums.Enums.LockMode.Parse(xmlReader.ReadString(), LockModeFormat.DisplayName);
                    break;
                case "Days":
                    response.LockRetainUntil = DateTimeOffset.UtcNow.AddDays(xmlReader.ReadElementContentAsInt());
                    break;
                case "Years":
                    response.LockRetainUntil = DateTimeOffset.UtcNow.AddYears(xmlReader.ReadElementContentAsInt());
                    break;
            }
        }
    }
}