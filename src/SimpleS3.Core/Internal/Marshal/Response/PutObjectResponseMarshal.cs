using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response
{
    [UsedImplicitly]
    internal class PutObjectResponseMarshal : IResponseMarshal<PutObjectRequest, PutObjectResponse>
    {
        public void MarshalResponse(PutObjectRequest request, PutObjectResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.StorageClass = headers.GetHeaderEnum<StorageClass>(AmzHeaders.XAmzStorageClass);

            //It should default to standard
            if (response.StorageClass == StorageClass.Unknown)
                response.StorageClass = StorageClass.Standard;

            response.ETag = headers.GetHeader(HttpHeaders.ETag);
            response.ExpiresOn = headers.GetHeader(AmzHeaders.XAmzExpiration);
            response.SseAlgorithm = headers.GetHeaderEnum<SseAlgorithm>(AmzHeaders.XAmzSSE);
            response.SseKmsKeyId = headers.GetHeader(AmzHeaders.XAmzSSEAwsKmsKeyId);
            response.SseCustomerAlgorithm = headers.GetHeaderEnum<SseCustomerAlgorithm>(AmzHeaders.XAmzSSECustomerAlgorithm);
            response.SseCustomerKeyMd5 = headers.GetHeaderByteArray(AmzHeaders.XAmzSSECustomerKeyMD5, BinaryEncoding.Base64);
        }
    }
}