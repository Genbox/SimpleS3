using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects.XML;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response
{
    [UsedImplicitly]
    internal class CompleteMultipartUploadResponseMarshal : IResponseMarshal<CompleteMultipartUploadRequest, CompleteMultipartUploadResponse>
    {
        public void MarshalResponse(IS3Config config, CompleteMultipartUploadRequest request, CompleteMultipartUploadResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.ExpiresOn = headers.GetHeader(AmzHeaders.XAmzExpiration);
            response.SseAlgorithm = headers.GetHeaderEnum<SseAlgorithm>(AmzHeaders.XAmzSSE);
            response.SseKmsKeyId = headers.GetHeader(AmzHeaders.XAmzSSEAwsKmsKeyId);
            response.VersionId = headers.GetHeader(AmzHeaders.XAmzVersionId);
            response.RequestCharged = string.Equals("RequestPayer", headers.GetHeader(AmzHeaders.XAmzVersionId), StringComparison.OrdinalIgnoreCase);

            //response.SseCustomerAlgorithm = headers.GetHeaderEnum<SseCustomerAlgorithm>(AmzHeaders.XAmzSSECustomerAlgorithm);
            //response.SseCustomerKeyMd5 = headers.GetHeaderByteArray(AmzHeaders.XAmzSSECustomerKeyMD5, BinaryEncoding.Base64);

            using (responseStream)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(CompleteMultipartUploadResult));

                using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
                {
                    xmlReader.Namespaces = false;

                    CompleteMultipartUploadResult resp = (CompleteMultipartUploadResult)xmlSerializer.Deserialize(xmlReader);
                    response.Location = resp.Location;
                    response.BucketName = resp.Bucket;
                    response.ObjectKey = resp.Key;
                    response.ETag = resp.ETag;
                }
            }
        }
    }
}