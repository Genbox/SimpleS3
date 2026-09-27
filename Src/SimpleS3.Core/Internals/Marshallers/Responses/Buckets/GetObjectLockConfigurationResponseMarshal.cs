using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class GetObjectLockConfigurationResponseMarshal : IResponseMarshal<GetObjectLockConfigurationResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetObjectLockConfigurationResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        using XmlReader xmlReader = XmlHelper.CreateReader(responseStream);
        if (!XmlHelper.ReadToDescendant(xmlReader, "ObjectLockConfiguration"))
            return;

        foreach (string name in XmlHelper.ReadChildren(xmlReader))
        {
            switch (name)
            {
                case "ObjectLockEnabled":
                    response.Enabled = xmlReader.ReadString().Trim() == "Enabled";
                    break;
                case "Rule":
                    ReadRule(response, xmlReader);
                    break;
            }
        }
    }

    private static void ReadRule(GetObjectLockConfigurationResponse response, XmlReader xmlReader)
    {
        foreach (string name in XmlHelper.ReadChildren(xmlReader))
        {
            if (name == "DefaultRetention")
                ReadDefaultRetention(response, xmlReader);
        }
    }

    private static void ReadDefaultRetention(GetObjectLockConfigurationResponse response, XmlReader xmlReader)
    {
        foreach (string name in XmlHelper.ReadChildren(xmlReader))
        {
            switch (name)
            {
                case "Mode":
                    response.LockMode = Core.Enums.Enums.LockMode.Parse(xmlReader.ReadString(), LockModeFormat.DisplayName);
                    break;
                case "Days":
                    response.LockRetainDays = XmlConvert.ToInt32(xmlReader.ReadString());
                    break;
                case "Years":
                    response.LockRetainYears = XmlConvert.ToInt32(xmlReader.ReadString());
                    break;
            }
        }
    }
}