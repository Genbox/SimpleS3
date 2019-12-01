using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using Genbox.SimpleS3.Core.Network.Responses.Multipart;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response.Multipart
{
    [UsedImplicitly]
    internal class UploadPartResponseMarshal : IResponseMarshal<UploadPartRequest, UploadPartResponse>
    {
        public void MarshalResponse(IS3Config config, UploadPartRequest request, UploadPartResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.PartNumber = request.PartNumber;
            response.ETag = headers.GetHeader(HttpHeaders.ETag);
            response.StorageClass = headers.GetHeaderEnum<StorageClass>(AmzHeaders.XAmzStorageClass);
            response.SseAlgorithm = headers.GetHeaderEnum<SseAlgorithm>(AmzHeaders.XAmzSse);
            response.SseKmsKeyId = headers.GetHeader(AmzHeaders.XAmzSseAwsKmsKeyId);
            response.SseCustomerAlgorithm = headers.GetHeaderEnum<SseCustomerAlgorithm>(AmzHeaders.XAmzSseCustomerAlgorithm);
            response.SseCustomerKeyMd5 = headers.GetHeaderByteArray(AmzHeaders.XAmzSseCustomerKeyMd5, BinaryEncoding.Base64);
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);
        }
    }
}