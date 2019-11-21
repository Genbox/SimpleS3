using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Core.Internal.Xml;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request.Object
{
    [UsedImplicitly]
    internal class DeleteObjectsRequestMarshal : IRequestMarshal<DeleteObjectsRequest>
    {
        public Stream MarshalRequest(DeleteObjectsRequest request, IS3Config config)
        {
            request.AddQueryParameter(AmzParameters.Delete, string.Empty);
            request.AddHeader(AmzHeaders.XAmzMfa, request.Mfa);
            request.AddHeader(AmzHeaders.XAmzBypassGovernanceRetention, request.BypassGovernanceRetention);
            request.AddHeader(AmzHeaders.XAmzRequestPayer, request.RequestPayer == Payer.Requester ? "requester" : null);

            FastXmlWriter xml = new FastXmlWriter(512);
            xml.WriteStartElement("Delete");

            if (request.Quiet)
                xml.WriteElement("Quiet", true);

            foreach (S3DeleteInfo info in request.Objects)
            {
                xml.WriteStartElement("Object");
                xml.WriteElement("Key", info.Name);

                if (!string.IsNullOrWhiteSpace(info.VersionId))
                    xml.WriteElement("VersionId", info.VersionId);

                xml.WriteEndElement("Object");
            }

            xml.WriteEndElement("Delete");

            byte[] data = xml.GetBytes();
            request.AddHeader(HttpHeaders.ContentMd5, CryptoHelper.Md5Hash(data), BinaryEncoding.Base64);
            return new MemoryStream(data);
        }
    }
}