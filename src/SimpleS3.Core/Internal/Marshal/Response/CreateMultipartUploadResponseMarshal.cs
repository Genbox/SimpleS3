using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects.XML;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response
{
    [UsedImplicitly]
    internal class CreateMultipartUploadResponseMarshal : IResponseMarshal<CreateMultipartUploadRequest, CreateMultipartUploadResponse>
    {
        public void MarshalResponse(CreateMultipartUploadRequest request, CreateMultipartUploadResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.AbortsOn = headers.GetHeaderDate(AmzHeaders.XAmzAbortDate, DateTimeFormat.Iso8601DateTimeExt);
            response.AbortRuleId = headers.GetHeader(AmzHeaders.XAmzAbortDate);
            response.SseAlgorithm = headers.GetHeaderEnum<SseAlgorithm>(AmzHeaders.XAmzSSE);
            response.SseKmsKeyId = headers.GetHeader(AmzHeaders.XAmzSSEAwsKmsKeyId);
            response.SseCustomerAlgorithm = headers.GetHeaderEnum<SseCustomerAlgorithm>(AmzHeaders.XAmzSSECustomerAlgorithm);
            response.SseCustomerKeyMd5 = headers.GetHeaderByteArray(AmzHeaders.XAmzSSECustomerKeyMD5, BinaryEncoding.Base64);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(InitiateMultipartUploadResult));

            using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
            {
                xmlReader.Namespaces = false;

                InitiateMultipartUploadResult resp = (InitiateMultipartUploadResult)xmlSerializer.Deserialize(xmlReader);
                response.Bucket = resp.Bucket;
                response.ObjectKey = resp.Key;
                response.UploadId = resp.UploadId;
            }
        }
    }
}