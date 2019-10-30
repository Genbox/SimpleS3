using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Constants;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request
{
    [UsedImplicitly]
    internal class UploadPartRequestMarshal : IRequestMarshal<UploadPartRequest>
    {
        public Stream MarshalRequest(UploadPartRequest request, IS3Config config)
        {
            request.AddQueryParameter(ObjectParameters.PartNumber, request.PartNumber);
            request.AddQueryParameter(ObjectParameters.UploadId, request.UploadId);
            request.AddHeader(HttpHeaders.ContentMd5, request.ContentMd5, BinaryEncoding.Base64);
            request.AddHeader(AmzHeaders.XAmzSSECustomerAlgorithm, request.SseCustomerAlgorithm);
            request.AddHeader(AmzHeaders.XAmzSSECustomerKey, request.SseCustomerKey, BinaryEncoding.Base64);
            request.AddHeader(AmzHeaders.XAmzSSECustomerKeyMD5, request.SseCustomerKeyMd5, BinaryEncoding.Base64);
            return request.Content;
        }
    }
}