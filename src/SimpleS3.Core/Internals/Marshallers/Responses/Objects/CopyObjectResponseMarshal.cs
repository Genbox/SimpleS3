using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects.Xml;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects
{
    [UsedImplicitly]
    internal class CopyObjectResponseMarshal : IResponseMarshal<CopyObjectResponse>
    {
        public void MarshalResponse(Config config, CopyObjectResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.NewVersionId = headers.GetHeader(AmzHeaders.XAmzCopySourceVersionId);

            if (HeaderParserHelper.TryParseExpiration(headers, out (DateTimeOffset expiresOn, string ruleId) data))
            {
                response.LifeCycleExpiresOn = data.expiresOn;
                response.LifeCycleRuleId = data.ruleId;
            }

            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

            response.SseAlgorithm = headers.GetHeaderEnum<SseAlgorithm>(AmzHeaders.XAmzSse);
            response.SseKmsKeyId = headers.GetHeader(AmzHeaders.XAmzSseAwsKmsKeyId);
            response.SseContext = headers.GetHeader(AmzHeaders.XAmzSseContext);

            response.SseCustomerAlgorithm = headers.GetHeaderEnum<SseCustomerAlgorithm>(AmzHeaders.XAmzSseCustomerAlgorithm);
            response.SseCustomerKeyMd5 = headers.GetHeaderByteArray(AmzHeaders.XAmzSseCustomerKeyMd5, BinaryEncoding.Base64);

            response.VersionId = headers.GetHeader(AmzHeaders.XAmzVersionId);

            XmlSerializer s = new XmlSerializer(typeof(CopyObjectResult));

            using (XmlTextReader r = new XmlTextReader(responseStream))
            {
                r.Namespaces = false;

                CopyObjectResult deleteResult = (CopyObjectResult)s.Deserialize(r);

                response.LastModified = deleteResult.LastModified;
                response.ETag = deleteResult.ETag;
            }
        }
    }
}