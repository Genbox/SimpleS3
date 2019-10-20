using System.IO;
using System.Text;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Constants;
using Genbox.SimpleS3.Core.Requests.Objects;
using Genbox.SimpleS3.Core.Requests.Objects.Types;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request
{
    [UsedImplicitly]
    internal class CompleteMultipartUploadRequestMarshal : IRequestMarshal<CompleteMultipartUploadRequest>
    {
        public Stream MarshalRequest(CompleteMultipartUploadRequest request, IS3Config config)
        {
            request.AddQueryParameter(ObjectParameters.UploadId, request.UploadId);

            //build the XML required to describe each part
            StringBuilder sb = new StringBuilder();
            sb.Append("<CompleteMultipartUpload>");

            foreach (S3PartInfo partInfo in request.UploadParts)
            {
                sb.Append("<Part>");
                sb.Append("<ETag>").Append(partInfo.ETag.Trim('"')).Append("</ETag>");
                sb.Append("<PartNumber>").Append(partInfo.PartNumber).Append("</PartNumber>");
                sb.Append("</Part>");
            }

            sb.Append("</CompleteMultipartUpload>");

            return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
        }
    }
}