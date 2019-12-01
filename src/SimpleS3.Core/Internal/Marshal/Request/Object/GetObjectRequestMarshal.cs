using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request.Object
{
    [UsedImplicitly]
    internal class GetObjectRequestMarshal : IRequestMarshal<GetObjectRequest>
    {
        public Stream MarshalRequest(GetObjectRequest request, IS3Config config)
        {
            request.AddHeader(AmzHeaders.XAmzRequestPayer, request.RequestPayer == Payer.Requester ? "requester" : null);
            request.AddHeader(HttpHeaders.Range, request.Range);
            request.AddHeader(HttpHeaders.IfMatch, request.IfETagMatch);
            request.AddHeader(HttpHeaders.IfNoneMatch, request.IfETagNotMatch);
            request.AddHeader(HttpHeaders.IfModifiedSince, request.IfModifiedSince, DateTimeFormat.Rfc1123);
            request.AddHeader(HttpHeaders.IfUnmodifiedSince, request.IfUnmodifiedSince, DateTimeFormat.Rfc1123);
            request.AddHeader(AmzHeaders.XAmzSseCustomerAlgorithm, request.SseCustomerAlgorithm);
            request.AddHeader(AmzHeaders.XAmzSseCustomerKey, request.SseCustomerKey, BinaryEncoding.Base64);
            request.AddHeader(AmzHeaders.XAmzSseCustomerKeyMd5, request.SseCustomerKeyMd5, BinaryEncoding.Base64);
            request.AddQueryParameter(AmzParameters.ResponseCacheControl, request.ResponseCacheControl);
            request.AddQueryParameter(AmzParameters.ResponseExpires, request.ResponseExpires, DateTimeFormat.Rfc1123);
            request.AddQueryParameter(AmzParameters.ResponseContentDisposition, request.ResponseContentDisposition);
            request.AddQueryParameter(AmzParameters.ResponseContentEncoding, request.ResponseContentEncoding);
            request.AddQueryParameter(AmzParameters.ResponseContentLanguage, request.ResponseContentLanguage);
            request.AddQueryParameter(AmzParameters.ResponseContentType, request.ResponseContentType);
            request.AddQueryParameter(AmzParameters.PartNumber, request.PartNumber);
            request.AddQueryParameter(AmzParameters.VersionId, request.VersionId);
            return null;
        }
    }
}