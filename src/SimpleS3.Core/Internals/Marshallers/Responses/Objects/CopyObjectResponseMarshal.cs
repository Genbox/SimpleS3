using System;
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
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;

internal class CopyObjectResponseMarshal : IResponseMarshal<CopyObjectResponse>
{
    public void MarshalResponse(SimpleS3Config config, CopyObjectResponse response, IDictionary<string, string> headers, Stream responseStream)
    {
        response.NewVersionId = headers.GetOptionalValue(AmzHeaders.XAmzCopySourceVersionId);

        if (ParserHelper.TryParseExpiration(headers, out (DateTimeOffset expiresOn, string ruleId) data))
        {
            response.LifeCycleExpiresOn = data.expiresOn;
            response.LifeCycleRuleId = data.ruleId;
        }

        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

        response.SseAlgorithm = headers.GetHeaderEnum<SseAlgorithm>(AmzHeaders.XAmzSse);
        response.SseKmsKeyId = headers.GetOptionalValue(AmzHeaders.XAmzSseAwsKmsKeyId);
        response.SseContext = headers.GetOptionalValue(AmzHeaders.XAmzSseContext);

        response.SseCustomerAlgorithm = headers.GetHeaderEnum<SseCustomerAlgorithm>(AmzHeaders.XAmzSseCustomerAlgorithm);
        response.SseCustomerKeyMd5 = headers.GetHeaderByteArray(AmzHeaders.XAmzSseCustomerKeyMd5, BinaryEncoding.Base64);

        response.VersionId = headers.GetOptionalValue(AmzHeaders.XAmzVersionId);

        using (XmlTextReader xmlReader = new XmlTextReader(responseStream))
        {
            xmlReader.ReadToDescendant("CopyObjectResult");

            foreach (string name in XmlHelper.ReadElements(xmlReader))
            {
                switch (name)
                {
                    case "ETag":
                        response.ETag = xmlReader.ReadString();
                        break;
                    case "LastModified":
                        response.LastModified = ValueHelper.ParseDate(xmlReader.ReadString(), DateTimeFormat.Iso8601DateTimeExt);
                        break;
                }
            }
        }
    }
}