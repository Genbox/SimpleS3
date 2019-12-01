using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request.Multipart
{
    [UsedImplicitly]
    internal class UploadPartRequestMarshal : IRequestMarshal<UploadPartRequest>
    {
        public Stream MarshalRequest(UploadPartRequest request, IS3Config config)
        {
            request.AddQueryParameter(AmzParameters.PartNumber, request.PartNumber);
            request.AddQueryParameter(AmzParameters.UploadId, request.UploadId);
            request.AddHeader(HttpHeaders.ContentMd5, request.ContentMd5, BinaryEncoding.Base64);
            request.AddHeader(AmzHeaders.XAmzSseCustomerAlgorithm, request.SseCustomerAlgorithm);
            request.AddHeader(AmzHeaders.XAmzSseCustomerKey, request.SseCustomerKey, BinaryEncoding.Base64);
            request.AddHeader(AmzHeaders.XAmzSseCustomerKeyMd5, request.SseCustomerKeyMd5, BinaryEncoding.Base64);
            request.AddHeader(AmzHeaders.XAmzRequestPayer, request.RequestPayer == Payer.Requester ? "requester" : null);

            return request.Content;
        }
    }
}