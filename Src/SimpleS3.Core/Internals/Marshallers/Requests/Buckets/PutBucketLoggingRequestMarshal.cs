using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Linq;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Buckets;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Buckets;

[SuppressMessage("Security", "S5332", Justification = "The HTTP URI is the namespace mandated by the S3 XML protocol, not a network endpoint.")]
internal sealed class PutBucketLoggingRequestMarshal : IRequestMarshal<PutBucketLoggingRequest>
{
    public Stream MarshalRequest(PutBucketLoggingRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter("logging", string.Empty);
        XNamespace ns = "http://s3.amazonaws.com/doc/2006-03-01/";
        XElement root = new XElement(ns + "BucketLoggingStatus");

        if (request.LoggingConfiguration != null)
            root.Add(CreateLoggingConfiguration(ns, request.LoggingConfiguration));

        return new MemoryStream(Encoding.UTF8.GetBytes(root.ToString(SaveOptions.DisableFormatting)));
    }

    private static XElement CreateLoggingConfiguration(XNamespace ns, S3LoggingConfiguration configuration)
    {
        XElement enabled = new XElement(ns + "LoggingEnabled",
            new XElement(ns + "TargetBucket", configuration.TargetBucket),
            new XElement(ns + "TargetPrefix", configuration.TargetPrefix ?? string.Empty));

        if (configuration.TargetGrants.Count > 0)
        {
            XElement grants = new XElement(ns + "TargetGrants");
            foreach (S3Grant grant in configuration.TargetGrants)
                grants.Add(CreateGrant(ns, grant));
            enabled.Add(grants);
        }

        if (configuration.TargetObjectKeyFormat.HasValue)
            enabled.Add(CreateObjectKeyFormat(ns, configuration.TargetObjectKeyFormat.Value));

        return enabled;
    }

    private static XElement CreateGrant(XNamespace ns, S3Grant grant)
    {
        if (grant.Grantee.Type is not (GrantType.CanonicalUser or GrantType.AmazonCustomerByEmail or GrantType.Group))
            throw new ArgumentOutOfRangeException(nameof(grant), grant.Grantee.Type, "Unknown logging grantee type");
        string permission = grant.Permission switch
        {
            S3Permission.FullControl => "FULL_CONTROL",
            S3Permission.Read => "READ",
            S3Permission.Write => "WRITE",
            _ => throw new ArgumentOutOfRangeException(nameof(grant), grant.Permission, "Unsupported logging grant permission")
        };
        XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
        XElement grantee = new XElement(ns + "Grantee",
            new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
            new XAttribute(xsi + "type", grant.Grantee.Type.ToString()));
        if (grant.Grantee.Id != null)
            grantee.Add(new XElement(ns + "ID", grant.Grantee.Id));
        if (grant.Grantee.DisplayName != null)
            grantee.Add(new XElement(ns + "DisplayName", grant.Grantee.DisplayName));
        if (grant.Grantee.EmailAddress != null)
            grantee.Add(new XElement(ns + "EmailAddress", grant.Grantee.EmailAddress));
        if (grant.Grantee.Uri != null)
            grantee.Add(new XElement(ns + "URI", grant.Grantee.Uri));
        return new XElement(ns + "Grant", grantee, new XElement(ns + "Permission", permission));
    }

    private static XElement CreateObjectKeyFormat(XNamespace ns, S3LoggingObjectKeyFormat format)
    {
        string? dateSource = format switch
        {
            S3LoggingObjectKeyFormat.SimplePrefix => null,
            S3LoggingObjectKeyFormat.PartitionedPrefixEventTime => "EventTime",
            S3LoggingObjectKeyFormat.PartitionedPrefixDeliveryTime => "DeliveryTime",
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Unknown logging object-key format")
        };

        if (format == S3LoggingObjectKeyFormat.SimplePrefix)
            return new XElement(ns + "TargetObjectKeyFormat", new XElement(ns + "SimplePrefix"));

        return new XElement(ns + "TargetObjectKeyFormat",
            new XElement(ns + "PartitionedPrefix", new XElement(ns + "PartitionDateSource", dateSource!)));
    }
}