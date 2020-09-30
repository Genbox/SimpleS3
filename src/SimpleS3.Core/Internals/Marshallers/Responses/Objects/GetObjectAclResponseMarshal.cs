using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects.Xml;
using Genbox.SimpleS3.Core.Network.Responses.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.XmlTypes;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects
{
    [UsedImplicitly]
    internal class GetObjectAclResponseMarshal : IResponseMarshal<GetObjectAclResponse>
    {
        public void MarshalResponse(IConfig config, GetObjectAclResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

            XmlSerializer s = new XmlSerializer(typeof(AccessControlPolicy));

            using (XmlTextReader r = new XmlTextReader(responseStream))
            {
                r.Namespaces = true;

                AccessControlPolicy aclOutput = (AccessControlPolicy)s.Deserialize(r);

                if (aclOutput.Owner != null)
                {
                    response.Owner = new S3Identity();
                    response.Owner.Id = aclOutput.Owner.Id;
                    response.Owner.Name = aclOutput.Owner.DisplayName;
                }

                if (aclOutput.AccessControlList?.Grants != null)
                {
                    response.Grants = new List<S3Grant>(aclOutput.AccessControlList.Grants.Sum(x => x.Grantee.Count));

                    foreach (Grant grant in aclOutput.AccessControlList.Grants)
                    {
                        foreach (GranteeBase grantee in grant.Grantee)
                        {
                            S3Grant s3Grant = new S3Grant();
                            s3Grant.Permission = ValueHelper.ParseEnum<Permission>(grant.Permission);

                            if (grantee is Group grantGroup)
                            {
                                s3Grant.Uri = grantGroup.Uri;
                                s3Grant.Type = GrantType.Group;
                            }
                            else if (grantee is CanonicalUser grantUser)
                            {
                                s3Grant.Id = grantUser.Id;
                                s3Grant.Name = grantUser.DisplayName;
                                s3Grant.Type = GrantType.User;
                            }

                            response.Grants.Add(s3Grant);
                        }
                    }
                }
                else
                    response.Grants = Array.Empty<S3Grant>();
            }
        }
    }
}