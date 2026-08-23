using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Internals.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

internal sealed class PutBucketWebsiteRequestMarshal : IRequestMarshal<PutBucketWebsiteRequest>
{
    public Stream? MarshalRequest(PutBucketWebsiteRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Website, string.Empty);
        FastXmlWriter writer = new FastXmlWriter(1024);
        S3WebsiteConfiguration website = request.WebsiteConfiguration;

        writer.WriteStartElement("WebsiteConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");

        if (website.RedirectAllRequestsTo != null)
            WriteRedirectAllRequestsTo(writer, website.RedirectAllRequestsTo);

        if (website.IndexDocumentSuffix != null)
        {
            writer.WriteStartElement("IndexDocument");
            writer.WriteElement("Suffix", website.IndexDocumentSuffix);
            writer.WriteEndElement("IndexDocument");
        }

        if (website.ErrorDocumentKey != null)
        {
            writer.WriteStartElement("ErrorDocument");
            writer.WriteElement("Key", website.ErrorDocumentKey);
            writer.WriteEndElement("ErrorDocument");
        }

        if (website.RoutingRules.Count > 0)
        {
            writer.WriteStartElement("RoutingRules");

            foreach (S3RoutingRule rule in website.RoutingRules)
                WriteRoutingRule(writer, rule);

            writer.WriteEndElement("RoutingRules");
        }

        writer.WriteEndElement("WebsiteConfiguration");
        return new MemoryStream(writer.GetBytes());
    }

    private static void WriteRedirectAllRequestsTo(FastXmlWriter writer, S3RedirectAllRequestsTo redirect)
    {
        writer.WriteStartElement("RedirectAllRequestsTo");
        writer.WriteElement("HostName", redirect.HostName);

        if (redirect.Protocol != null)
            writer.WriteElement("Protocol", redirect.Protocol);

        writer.WriteEndElement("RedirectAllRequestsTo");
    }

    private static void WriteRoutingRule(FastXmlWriter writer, S3RoutingRule rule)
    {
        writer.WriteStartElement("RoutingRule");

        if (rule.Condition != null)
        {
            writer.WriteStartElement("Condition");

            if (rule.Condition.KeyPrefixEquals != null)
                writer.WriteElement("KeyPrefixEquals", rule.Condition.KeyPrefixEquals);

            if (rule.Condition.HttpErrorCodeReturnedEquals != null)
                writer.WriteElement("HttpErrorCodeReturnedEquals", rule.Condition.HttpErrorCodeReturnedEquals);

            writer.WriteEndElement("Condition");
        }

        WriteRedirect(writer, rule.Redirect);
        writer.WriteEndElement("RoutingRule");
    }

    private static void WriteRedirect(FastXmlWriter writer, S3Redirect redirect)
    {
        writer.WriteStartElement("Redirect");

        if (redirect.HostName != null)
            writer.WriteElement("HostName", redirect.HostName);

        if (redirect.Protocol != null)
            writer.WriteElement("Protocol", redirect.Protocol);

        if (redirect.ReplaceKeyPrefixWith != null)
            writer.WriteElement("ReplaceKeyPrefixWith", redirect.ReplaceKeyPrefixWith);

        if (redirect.ReplaceKeyWith != null)
            writer.WriteElement("ReplaceKeyWith", redirect.ReplaceKeyWith);

        if (redirect.HttpRedirectCode != null)
            writer.WriteElement("HttpRedirectCode", redirect.HttpRedirectCode);

        writer.WriteEndElement("Redirect");
    }
}