using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects;

internal sealed class PutObjectResponseMarshal : IResponseMarshal<PutObjectResponse>
{
    public void MarshalResponse(SimpleS3Config config, PutObjectResponse response, IDictionary<string, string> headers, Stream responseStream)
    {
        if (headers.TryGetHeader(AmzHeaders.XAmzStorageClass, out string? storageClass))
            response.StorageClass = Core.Enums.Enums.StorageClass.Parse(storageClass, StorageClassFormat.DisplayName);

        //It should default to standard
        if (response.StorageClass == StorageClass.Unknown)
            response.StorageClass = StorageClass.Standard;

        response.ETag = headers.GetOptionalValue(HttpHeaders.ETag);

        if (headers.TryGetHeader(AmzHeaders.XAmzSse, out string? sseHeader))
            response.SseAlgorithm = Core.Enums.Enums.SseAlgorithm.Parse(sseHeader, SseAlgorithmFormat.DisplayName);

        response.SseKmsKeyId = headers.GetOptionalValue(AmzHeaders.XAmzSseAwsKmsKeyId);

        if (headers.TryGetHeader(AmzHeaders.XAmzSseCustomerAlgorithm, out string? sseAlgorithm))
            response.SseCustomerAlgorithm = Core.Enums.Enums.SseCustomerAlgorithm.Parse(sseAlgorithm, SseCustomerAlgorithmFormat.DisplayName);

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