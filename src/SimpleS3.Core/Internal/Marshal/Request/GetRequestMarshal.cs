using System.IO;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Constants;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Requests.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request
{
    [UsedImplicitly]
    internal class GetRequestMarshal : IRequestMarshal<GetObjectRequest>
    {
        public Stream MarshalRequest(GetObjectRequest request)
        {
            request.AddHeader(HttpHeaders.Range, request.Range);
            request.AddHeader(HttpHeaders.IfMatch, request.IfETagMatch);
            request.AddHeader(HttpHeaders.IfNoneMatch, request.IfETagNotMatch);
            request.AddHeader(HttpHeaders.IfModifiedSince, request.IfModifiedSince, DateTimeFormat.Rfc1123);
            request.AddHeader(HttpHeaders.IfUnmodifiedSince, request.IfUnmodifiedSince, DateTimeFormat.Rfc1123);
            request.AddHeader(AmzHeaders.XAmzSSECustomerAlgorithm, request.SseCustomerAlgorithm);
            request.AddHeader(AmzHeaders.XAmzSSECustomerKey, request.SseCustomerKey, BinaryEncoding.Base64);
            request.AddHeader(AmzHeaders.XAmzSSECustomerKeyMD5, request.SseCustomerKeyMd5, BinaryEncoding.Base64);
            request.AddQueryParameter(ObjectParameters.ResponseCacheControl, request.ResponseCacheControl);
            request.AddQueryParameter(ObjectParameters.ResponseExpires, request.ResponseExpires, DateTimeFormat.Rfc1123);
            request.AddQueryParameter(ObjectParameters.ResponseContentDisposition, request.ResponseContentDisposition);
            request.AddQueryParameter(ObjectParameters.ResponseContentEncoding, request.ResponseContentEncoding);
            request.AddQueryParameter(ObjectParameters.ResponseContentLanguage, request.ResponseContentLanguage);
            request.AddQueryParameter(ObjectParameters.ResponseContentType, request.ResponseContentType);
            request.AddQueryParameter(ObjectParameters.PartNumber, request.PartNumber);
            request.AddQueryParameter(ObjectParameters.VersionId, request.VersionId);
            return null;
        }
    }
}