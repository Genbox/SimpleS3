using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Multipart
{
    [UsedImplicitly]
    internal class CompleteMultipartUploadResponseMarshal : IResponseMarshal<CompleteMultipartUploadResponse>
    {
        public void MarshalResponse(Config config, CompleteMultipartUploadResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.VersionId = headers.GetOptionalValue(AmzHeaders.XAmzVersionId);
            response.SseAlgorithm = headers.GetHeaderEnum<SseAlgorithm>(AmzHeaders.XAmzSse);
            response.SseKmsKeyId = headers.GetOptionalValue(AmzHeaders.XAmzSseAwsKmsKeyId);
            response.SseCustomerAlgorithm = headers.GetHeaderEnum<SseCustomerAlgorithm>(AmzHeaders.XAmzSseCustomerAlgorithm);
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

            if (HeaderParserHelper.TryParseExpiration(headers, out (DateTimeOffset expiresOn, string ruleId) data))
            {
                response.LifeCycleExpiresOn = data.expiresOn;
                response.LifeCycleRuleId = data.ruleId;
            }

            using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
            {
                xmlReader.ReadToDescendant("CompleteMultipartUploadResult");

                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType != XmlNodeType.Element)
                        continue;

                    switch (xmlReader.Name)
                    {
                        case "Location":
                            response.Location = xmlReader.ReadString();
                            break;
                        case "Bucket":
                            response.BucketName = xmlReader.ReadString();
                            break;
                        case "Key":
                            response.ObjectKey = xmlReader.ReadString();
                            break;
                        case "ETag":
                            response.ETag = xmlReader.ReadString();
                            break;
                    }
                }
            }
        }
    }
}