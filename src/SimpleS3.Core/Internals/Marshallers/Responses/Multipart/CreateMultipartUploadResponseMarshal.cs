using System.Collections.Generic;
using System.IO;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Multipart
{
    internal class CreateMultipartUploadResponseMarshal : IResponseMarshal<CreateMultipartUploadResponse>
    {
        public void MarshalResponse(Config config, CreateMultipartUploadResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.AbortsOn = headers.GetHeaderDate(AmzHeaders.XAmzAbortDate, DateTimeFormat.Rfc1123);
            response.AbortRuleId = headers.GetOptionalValue(AmzHeaders.XAmzAbortRuleId);
            response.SseAlgorithm = headers.GetHeaderEnum<SseAlgorithm>(AmzHeaders.XAmzSse);
            response.SseKmsKeyId = headers.GetOptionalValue(AmzHeaders.XAmzSseAwsKmsKeyId);
            response.SseCustomerAlgorithm = headers.GetHeaderEnum<SseCustomerAlgorithm>(AmzHeaders.XAmzSseCustomerAlgorithm);
            response.SseCustomerKeyMd5 = headers.GetHeaderByteArray(AmzHeaders.XAmzSseCustomerKeyMd5, BinaryEncoding.Base64);
            response.SseContext = headers.GetOptionalValue(AmzHeaders.XAmzSseContext);
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

            using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
            {
                xmlReader.ReadToDescendant("InitiateMultipartUploadResult");

                foreach (string name in XmlHelper.ReadElements(xmlReader))
                {
                    switch (name)
                    {
                        case "Bucket":
                            response.BucketName = xmlReader.ReadString();
                            break;
                        case "Key":
                            response.ObjectKey = xmlReader.ReadString();
                            break;
                        case "UploadId":
                            response.UploadId = xmlReader.ReadString();
                            break;
                    }
                }
            }
        }
    }
}