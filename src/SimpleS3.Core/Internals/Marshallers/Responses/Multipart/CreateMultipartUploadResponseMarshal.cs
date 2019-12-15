using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Marshallers;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart.Xml;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Multipart
{
    [UsedImplicitly]
    internal class CreateMultipartUploadResponseMarshal : IResponseMarshal<CreateMultipartUploadRequest, CreateMultipartUploadResponse>
    {
        public void MarshalResponse(IS3Config config, CreateMultipartUploadRequest request, CreateMultipartUploadResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.AbortsOn = headers.GetHeaderDate(AmzHeaders.XAmzAbortDate, DateTimeFormat.Rfc1123);
            response.AbortRuleId = headers.GetHeader(AmzHeaders.XAmzAbortRuleId);
            response.SseAlgorithm = headers.GetHeaderEnum<SseAlgorithm>(AmzHeaders.XAmzSse);
            response.SseKmsKeyId = headers.GetHeader(AmzHeaders.XAmzSseAwsKmsKeyId);
            response.SseCustomerAlgorithm = headers.GetHeaderEnum<SseCustomerAlgorithm>(AmzHeaders.XAmzSseCustomerAlgorithm);
            response.SseCustomerKeyMd5 = headers.GetHeaderByteArray(AmzHeaders.XAmzSseCustomerKeyMd5, BinaryEncoding.Base64);
            response.SseContext = headers.GetHeader(AmzHeaders.XAmzSseContext);
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