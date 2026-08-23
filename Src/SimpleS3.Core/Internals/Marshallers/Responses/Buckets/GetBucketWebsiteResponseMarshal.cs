using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Buckets;

internal sealed class GetBucketWebsiteResponseMarshal : IResponseMarshal<GetBucketWebsiteResponse>
{
    public void MarshalResponse(SimpleS3Config config, GetBucketWebsiteResponse response, IDictionary<string, string> headers, ContentStream responseStream)
    {
        using XmlReader reader = XmlHelper.CreateReader(responseStream);
        reader.ReadToDescendant("WebsiteConfiguration");
        response.WebsiteConfiguration = ParseWebsite(reader);
    }

    private static S3WebsiteConfiguration ParseWebsite(XmlReader reader)
    {
        S3WebsiteConfiguration website = new S3WebsiteConfiguration();

        foreach (string name in XmlHelper.ReadElements(reader, "WebsiteConfiguration"))
        {
            switch (name)
            {
                case "IndexDocument":
                    website.IndexDocumentSuffix = ReadChildValue(reader, "IndexDocument", "Suffix");
                    break;
                case "ErrorDocument":
                    website.ErrorDocumentKey = ReadChildValue(reader, "ErrorDocument", "Key");
                    break;
                case "RedirectAllRequestsTo":
                    website.RedirectAllRequestsTo = ParseRedirectAllRequestsTo(reader);
                    break;
                case "RoutingRules":
                    ParseRoutingRules(website, reader);
                    break;
            }
        }

        return website;
    }

    private static string? ReadChildValue(XmlReader reader, string parent, string child)
    {
        foreach (string name in XmlHelper.ReadElements(reader, parent))
        {
            if (name == child)
                return reader.ReadString();
        }

        return null;
    }

    private static S3RedirectAllRequestsTo ParseRedirectAllRequestsTo(XmlReader reader)
    {
        string? hostName = null;
        string? protocol = null;

        foreach (string name in XmlHelper.ReadElements(reader, "RedirectAllRequestsTo"))
        {
            switch (name)
            {
                case "HostName":
                    hostName = reader.ReadString();
                    break;
                case "Protocol":
                    protocol = reader.ReadString();
                    break;
            }
        }

        if (hostName == null)
            throw new InvalidOperationException("Missing required values");

        return new S3RedirectAllRequestsTo(hostName) { Protocol = protocol };
    }

    private static void ParseRoutingRules(S3WebsiteConfiguration website, XmlReader reader)
    {
        foreach (string name in XmlHelper.ReadElements(reader, "RoutingRules"))
        {
            if (name == "RoutingRule")
                website.RoutingRules.Add(ParseRoutingRule(reader));
        }
    }

    private static S3RoutingRule ParseRoutingRule(XmlReader reader)
    {
        S3RoutingRuleCondition? condition = null;
        S3Redirect? redirect = null;

        foreach (string name in XmlHelper.ReadElements(reader, "RoutingRule"))
        {
            switch (name)
            {
                case "Condition":
                    condition = ParseCondition(reader);
                    break;
                case "Redirect":
                    redirect = ParseRedirect(reader);
                    break;
            }
        }

        if (redirect == null)
            throw new InvalidOperationException("Missing required values");

        return new S3RoutingRule(redirect) { Condition = condition };
    }

    private static S3RoutingRuleCondition ParseCondition(XmlReader reader)
    {
        S3RoutingRuleCondition condition = new S3RoutingRuleCondition();

        foreach (string name in XmlHelper.ReadElements(reader, "Condition"))
        {
            switch (name)
            {
                case "KeyPrefixEquals":
                    condition.KeyPrefixEquals = reader.ReadString();
                    break;
                case "HttpErrorCodeReturnedEquals":
                    condition.HttpErrorCodeReturnedEquals = reader.ReadString();
                    break;
            }
        }

        return condition;
    }

    private static S3Redirect ParseRedirect(XmlReader reader)
    {
        S3Redirect redirect = new S3Redirect();

        foreach (string name in XmlHelper.ReadElements(reader, "Redirect"))
        {
            switch (name)
            {
                case "HostName":
                    redirect.HostName = reader.ReadString();
                    break;
                case "Protocol":
                    redirect.Protocol = reader.ReadString();
                    break;
                case "ReplaceKeyPrefixWith":
                    redirect.ReplaceKeyPrefixWith = reader.ReadString();
                    break;
                case "ReplaceKeyWith":
                    redirect.ReplaceKeyWith = reader.ReadString();
                    break;
                case "HttpRedirectCode":
                    redirect.HttpRedirectCode = reader.ReadString();
                    break;
            }
        }

        return redirect;
    }
}