using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Multipart
{
    internal class UploadPartResponseMarshal : IResponseMarshal<UploadPartResponse>
    {
        public void MarshalResponse(Config config, UploadPartResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.ETag = headers.GetRequiredValue(HttpHeaders.ETag);
            response.StorageClass = headers.GetHeaderEnum<StorageClass>(AmzHeaders.XAmzStorageClass);
            response.SseAlgorithm = headers.GetHeaderEnum<SseAlgorithm>(AmzHeaders.XAmzSse);
            response.SseKmsKeyId = headers.GetOptionalValue(AmzHeaders.XAmzSseAwsKmsKeyId);
            response.SseCustomerAlgorithm = headers.GetHeaderEnum<SseCustomerAlgorithm>(AmzHeaders.XAmzSseCustomerAlgorithm);
            response.SseCustomerKeyMd5 = headers.GetHeaderByteArray(AmzHeaders.XAmzSseCustomerKeyMd5, BinaryEncoding.Base64);
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);
        }
    }
}