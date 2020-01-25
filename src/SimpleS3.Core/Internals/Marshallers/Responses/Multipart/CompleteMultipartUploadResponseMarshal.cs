using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Marshallers;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart.Xml;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Multipart
{
    [UsedImplicitly]
    internal class CompleteMultipartUploadResponseMarshal : IResponseMarshal<CompleteMultipartUploadRequest, CompleteMultipartUploadResponse>
    {
        public void MarshalResponse(IConfig config, CompleteMultipartUploadRequest request, CompleteMultipartUploadResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.VersionId = headers.GetHeader(AmzHeaders.XAmzVersionId);
            response.SseAlgorithm = headers.GetHeaderEnum<SseAlgorithm>(AmzHeaders.XAmzSse);
            response.SseKmsKeyId = headers.GetHeader(AmzHeaders.XAmzSseAwsKmsKeyId);
            response.SseCustomerAlgorithm = headers.GetHeaderEnum<SseCustomerAlgorithm>(AmzHeaders.XAmzSseCustomerAlgorithm);
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

            if (HeaderParserHelper.TryParseExpiration(headers, out (DateTimeOffset expiresOn, string ruleId) data))
            {
                response.LifeCycleExpiresOn = data.expiresOn;
                response.LifeCycleRuleId = data.ruleId;
            }

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