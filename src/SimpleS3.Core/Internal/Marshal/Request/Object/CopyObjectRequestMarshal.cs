using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request.Object
{
    [UsedImplicitly]
    internal class CopyObjectRequestMarshal : IRequestMarshal<CopyObjectRequest>
    {
        public Stream MarshalRequest(CopyObjectRequest request, IS3Config config)
        {
            request.AddHeader(AmzHeaders.XAmzCopySource, '/' + request.SourceBucketName + '/' + request.SourceObjectKey);
            request.AddHeader(AmzHeaders.XAmzCopySourceIfMatch, request.IfETagMatch);
            request.AddHeader(AmzHeaders.XAmzCopySourceIfNoneMatch, request.IfETagNotMatch);
            request.AddHeader(AmzHeaders.XAmzCopySourceIfModifiedSince, request.IfModifiedSince, DateTimeFormat.Rfc1123);
            request.AddHeader(AmzHeaders.XAmzCopySourceIfUnmodifiedSince, request.IfUnmodifiedSince, DateTimeFormat.Rfc1123);
            request.AddHeader(AmzHeaders.XAmzCopySourceSseCustomerAlgorithm, request.SseCustomerAlgorithm);
            request.AddHeader(AmzHeaders.XAmzCopySourceSseCustomerKey, request.SseCustomerKey, BinaryEncoding.Base64);
            request.AddHeader(AmzHeaders.XAmzCopySourceSseCustomerKeyMd5, request.SseCustomerKeyMd5, BinaryEncoding.Base64);

            request.AddHeader(AmzHeaders.XAmzMetadataDirective, request.MetadataDirective);
            request.AddHeader(AmzHeaders.XAmzTaggingDirective, request.TaggingDirective);
            return null;
        }
    }
}