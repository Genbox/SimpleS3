using System;
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
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects.Xml;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response.Object
{
    [UsedImplicitly]
    internal class CopyObjectResponseMarshal : IResponseMarshal<CopyObjectRequest, CopyObjectResponse>
    {
        public void MarshalResponse(IS3Config config, CopyObjectRequest request, CopyObjectResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.NewVersionId = headers.GetHeader(AmzHeaders.XAmzCopySourceVersionId);

            if (HeaderParserHelper.TryParseExpiration(headers, out (DateTimeOffset expiresOn, string ruleId) data))
            {
                response.LifeCycleExpiresOn = data.expiresOn;
                response.LifeCycleRuleId = data.ruleId;
            }

            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

            response.SseAlgorithm = headers.GetHeaderEnum<SseAlgorithm>(AmzHeaders.XAmzSSE);
            response.SseKmsKeyId = headers.GetHeader(AmzHeaders.XAmzSSEAwsKmsKeyId);
            response.SseContext = headers.GetHeader(AmzHeaders.XAmzSSEContext);

            response.SseCustomerAlgorithm = headers.GetHeaderEnum<SseCustomerAlgorithm>(AmzHeaders.XAmzSSECustomerAlgorithm);
            response.SseCustomerKeyMd5 = headers.GetHeaderByteArray(AmzHeaders.XAmzSSECustomerKeyMD5, BinaryEncoding.Base64);

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