using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart.Xml;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response.Multipart
{
    [UsedImplicitly]
    internal class CreateMultipartUploadResponseMarshal : IResponseMarshal<CreateMultipartUploadRequest, CreateMultipartUploadResponse>
    {
        public void MarshalResponse(IS3Config config, CreateMultipartUploadRequest request, CreateMultipartUploadResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.AbortsOn = headers.GetHeaderDate(AmzHeaders.XAmzAbortDate, DateTimeFormat.Rfc1123);
            response.AbortRuleId = headers.GetHeader(AmzHeaders.XAmzAbortRuleId);
            response.SseAlgorithm = headers.GetHeaderEnum<SseAlgorithm>(AmzHeaders.XAmzSSE);
            response.SseKmsKeyId = headers.GetHeader(AmzHeaders.XAmzSSEAwsKmsKeyId);
            response.SseCustomerAlgorithm = headers.GetHeaderEnum<SseCustomerAlgorithm>(AmzHeaders.XAmzSSECustomerAlgorithm);
            response.SseCustomerKeyMd5 = headers.GetHeaderByteArray(AmzHeaders.XAmzSSECustomerKeyMD5, BinaryEncoding.Base64);
            response.SseContext = headers.GetHeader(AmzHeaders.XAmzSSEContext);
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

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