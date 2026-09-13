using System.Text;
using System.Xml.Linq;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects;

internal sealed class PutObjectAclRequestMarshal : IRequestMarshal<PutObjectAclRequest>
{
    public Stream? MarshalRequest(PutObjectAclRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Acl, string.Empty);

        bool hasAccessControlPolicy = request.Owner != null || request.Grants.Count != 0;
        bool hasAmazonGrantHeaders = HasAmazonGrantHeaders(request);

        RejectAccessControlPolicyConflicts(request, hasAccessControlPolicy, hasAmazonGrantHeaders);
        return hasAccessControlPolicy ? MarshalAccessControlPolicy(request) : null;
    }

    private static MemoryStream MarshalAccessControlPolicy(PutObjectAclRequest request)
    {
        XNamespace ns = "http://s3.amazonaws.com/doc/2006-03-01/";
        XElement root = new XElement(ns + "AccessControlPolicy");

        if (request.Owner != null)
        {
            XElement owner = new XElement(ns + "Owner", new XElement(ns + "ID", request.Owner.Id));
            if (request.Owner.Name != null)
                owner.Add(new XElement(ns + "DisplayName", request.Owner.Name));
            root.Add(owner);
        }

        XElement accessControlList = new XElement(ns + "AccessControlList");
        foreach (S3Grant grant in request.Grants)
            accessControlList.Add(CreateGrant(ns, grant));
        root.Add(accessControlList);

        return new MemoryStream(Encoding.UTF8.GetBytes(root.ToString(SaveOptions.DisableFormatting)));
    }

    private static void RejectAccessControlPolicyConflicts(PutObjectAclRequest request, bool hasAccessControlPolicy, bool hasAmazonGrantHeaders)
    {
        if (!hasAccessControlPolicy)
            return;

        if (request.Acl != ObjectCannedAcl.Unknown || request.Headers.ContainsKey(AmzHeaders.XAmzAcl))
            throw new InvalidOperationException("A predefined ACL cannot be combined with an XML access-control policy.");
        if (hasAmazonGrantHeaders)
            throw new InvalidOperationException("Amazon ACL grant headers cannot be combined with an XML access-control policy.");
    }

    private static bool HasAmazonGrantHeaders(PutObjectAclRequest request) =>
        request.AclGrantRead.HasData() || request.AclGrantReadAcp.HasData() || request.AclGrantWriteAcp.HasData() || request.AclGrantFullControl.HasData() ||
        request.Headers.ContainsKey(AmzHeaders.XAmzGrantRead) || request.Headers.ContainsKey(AmzHeaders.XAmzGrantReadAcp) || request.Headers.ContainsKey(AmzHeaders.XAmzGrantWriteAcp) || request.Headers.ContainsKey(AmzHeaders.XAmzGrantFullControl);

    private static XElement CreateGrant(XNamespace ns, S3Grant grant)
    {
        XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
        XElement grantee = new XElement(ns + "Grantee",
            new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
            new XAttribute(xsi + "type", grant.Grantee.Type.ToString()));

        switch (grant.Grantee.Type)
        {
            case GrantType.CanonicalUser:
                grantee.Add(new XElement(ns + "ID", Required(grant.Grantee.Id, grant.Grantee.Type)));
                if (grant.Grantee.DisplayName != null)
                    grantee.Add(new XElement(ns + "DisplayName", grant.Grantee.DisplayName));
                break;
            case GrantType.AmazonCustomerByEmail:
                grantee.Add(new XElement(ns + "EmailAddress", Required(grant.Grantee.EmailAddress, grant.Grantee.Type)));
                break;
            case GrantType.Group:
                grantee.Add(new XElement(ns + "URI", Required(grant.Grantee.Uri, grant.Grantee.Type)));
                break;
            case GrantType.Unknown:
            default:
                throw new NotSupportedException($"The {grant.Grantee.Type} grant type cannot be represented in Amazon S3 ACL XML.");
        }

        string permission = grant.Permission switch
        {
            S3Permission.FullControl => "FULL_CONTROL",
            S3Permission.Read => "READ",
            S3Permission.Write => "WRITE",
            S3Permission.ReadAcp => "READ_ACP",
            S3Permission.WriteAcp => "WRITE_ACP",
            _ => throw new NotSupportedException($"The {grant.Permission} permission cannot be represented in Amazon S3 ACL XML.")
        };

        return new XElement(ns + "Grant", grantee, new XElement(ns + "Permission", permission));
    }

    private static string Required(string? value, GrantType type) => value ?? throw new InvalidOperationException($"The {type} grant type is missing its required identifier.");
}