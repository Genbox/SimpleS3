using System.IO;
using System.Text;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Constants;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Internal.Helpers;
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
            request.AddQueryParameter(ObjectParameters.Delete, string.Empty);
            request.AddHeader(AmzHeaders.XAmzMfa, request.Mfa);
            request.AddHeader(AmzHeaders.XAmzBypassGovernanceRetention, request.BypassGovernanceRetention);

            StringBuilder sb = new StringBuilder(512);
            sb.Append("<Delete>");

            if (request.Quiet)
                sb.Append("<Quiet>true</Quiet>");

            foreach (S3DeleteInfo info in request.Objects)
            {
                sb.Append("<Object>");
                sb.Append("<Key>").Append(info.Name).Append("</Key>");

                if (!string.IsNullOrWhiteSpace(info.VersionId))
                    sb.Append("<VersionId>").Append(info.VersionId).Append("</VersionId>");

                sb.Append("</Object>");
            }

            sb.Append("</Delete>");

            byte[] data = Encoding.UTF8.GetBytes(sb.ToString());

            request.AddHeader(HttpHeaders.ContentMd5, CryptoHelper.Md5Hash(data), BinaryEncoding.Base64);

            return new MemoryStream(data);
        }
    }
}