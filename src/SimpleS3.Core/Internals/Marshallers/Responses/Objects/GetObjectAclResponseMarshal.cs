using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Core.Network.Xml;
using Genbox.SimpleS3.Core.Network.XmlTypes;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects
{
    [UsedImplicitly]
    internal class GetObjectAclResponseMarshal : IResponseMarshal<GetObjectAclResponse>
    {
        public void MarshalResponse(Config config, GetObjectAclResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

            XmlSerializer s = new XmlSerializer(typeof(AccessControlPolicy));

            using (XmlTextReader r = new XmlTextReader(responseStream))
            {
                r.Namespaces = true;

                AccessControlPolicy aclOutput = (AccessControlPolicy)s.Deserialize(r);

                if (aclOutput.Owner != null)
                    response.Owner = new S3Identity(aclOutput.Owner.Id, aclOutput.Owner.DisplayName);

                if (aclOutput.AccessControlList?.Grants != null)
                {
                    response.Grants = new List<S3Grant>(aclOutput.AccessControlList.Grants.Sum(x => x.Grantee.Count));

                    foreach (Grant grant in aclOutput.AccessControlList.Grants)
                    {
                        foreach (GranteeBase grantee in grant.Grantee)
                        {
                            Permission permission = ValueHelper.ParseEnum<Permission>(grant.Permission);
                            GrantType type;
                            string? id = null;
                            string? name = null;
                            string? uri = null;

                            if (grantee is Group grantGroup)
                            {
                                uri = grantGroup.Uri;
                                type = GrantType.Group;
                            }
                            else if (grantee is CanonicalUser grantUser)
                            {
                                id = grantUser.Id;
                                name = grantUser.DisplayName;
                                type = GrantType.User;
                            }
                            else
                                throw new InvalidOperationException("Unsupported user type");

                            response.Grants.Add(new S3Grant(type, permission, name, id, uri));
                        }
                    }
                }
                else
                    response.Grants = Array.Empty<S3Grant>();
            }
        }
    }
}