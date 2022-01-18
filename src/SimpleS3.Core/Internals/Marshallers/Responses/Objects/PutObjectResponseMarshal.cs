using System;
using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;

internal class PutObjectResponseMarshal : IResponseMarshal<PutObjectResponse>
{
    public void MarshalResponse(SimpleS3Config config, PutObjectResponse response, IDictionary<string, string> headers, Stream responseStream)
    {
        response.StorageClass = headers.GetHeaderEnum<StorageClass>(AmzHeaders.XAmzStorageClass);

        //It should default to standard
        if (response.StorageClass == StorageClass.Unknown)
            response.StorageClass = StorageClass.Standard;

        response.ETag = headers.GetOptionalValue(HttpHeaders.ETag);
        response.SseAlgorithm = headers.GetHeaderEnum<SseAlgorithm>(AmzHeaders.XAmzSse);
        response.SseKmsKeyId = headers.GetOptionalValue(AmzHeaders.XAmzSseAwsKmsKeyId);
        response.SseCustomerAlgorithm = headers.GetHeaderEnum<SseCustomerAlgorithm>(AmzHeaders.XAmzSseCustomerAlgorithm);
        response.SseCustomerKeyMd5 = headers.GetHeaderByteArray(AmzHeaders.XAmzSseCustomerKeyMd5, BinaryEncoding.Base64);
        response.VersionId = headers.GetOptionalValue(AmzHeaders.XAmzVersionId);
        response.SseContext = headers.GetOptionalValue(AmzHeaders.XAmzSseContext);
        response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

        if (ParserHelper.TryParseExpiration(headers, out (DateTimeOffset expiresOn, string ruleId) data))
        {
            response.LifeCycleExpiresOn = data.expiresOn;
            response.LifeCycleRuleId = data.ruleId;
        }
    }
}