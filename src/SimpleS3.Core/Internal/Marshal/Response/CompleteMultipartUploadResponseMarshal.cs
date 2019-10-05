using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Requests.Objects;
using Genbox.SimpleS3.Core.Responses.Objects;
using Genbox.SimpleS3.Core.Responses.Objects.XML;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response
{
    [UsedImplicitly]
    internal class CompleteMultipartUploadResponseMarshal : IResponseMarshal<CompleteMultipartUploadRequest, CompleteMultipartUploadResponse>
    {
        public void MarshalResponse(CompleteMultipartUploadRequest request, CompleteMultipartUploadResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.Expiration = headers.GetHeaderDate(AmzHeaders.XAmzExpiration, DateTimeFormat.Iso8601DateTimeExt);
            response.SseAlgorithm = headers.GetHeaderEnum<SseAlgorithm>(AmzHeaders.XAmzSSE);
            response.SseKmsKeyId = headers.GetHeader(AmzHeaders.XAmzSSEAwsKmsKeyId);
            response.SseCustomerAlgorithm = headers.GetHeaderEnum<SseCustomerAlgorithm>(AmzHeaders.XAmzSSECustomerAlgorithm);
            response.SseCustomerKeyMd5 = headers.GetHeaderByteArray(AmzHeaders.XAmzSSECustomerKeyMD5, BinaryEncoding.Base64);

            using (responseStream)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(CompleteMultipartUploadResult));

                using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
                {
                    xmlReader.Namespaces = false;

                    CompleteMultipartUploadResult resp = (CompleteMultipartUploadResult)xmlSerializer.Deserialize(xmlReader);
                    response.Location = resp.Location;
                    response.Bucket = resp.Bucket;
                    response.Key = resp.Key;
                    response.ETag = resp.ETag;
                }
            }
        }
    }
}