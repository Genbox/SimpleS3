using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects
{
    internal class CopyObjectRequestMarshal : IRequestMarshal<CopyObjectRequest>
    {
        public Stream? MarshalRequest(CopyObjectRequest request, SimpleS3Config config)
        {
            request.SetHeader(AmzHeaders.XAmzCopySource, '/' + request.SourceBucketName + '/' + request.SourceObjectKey);
            request.SetHeader(AmzHeaders.XAmzCopySourceIfMatch, request.IfETagMatch);
            request.SetHeader(AmzHeaders.XAmzCopySourceIfNoneMatch, request.IfETagNotMatch);
            request.SetHeader(AmzHeaders.XAmzCopySourceIfModifiedSince, request.IfModifiedSince, DateTimeFormat.Rfc1123);
            request.SetHeader(AmzHeaders.XAmzCopySourceIfUnmodifiedSince, request.IfUnmodifiedSince, DateTimeFormat.Rfc1123);
            request.SetHeader(AmzHeaders.XAmzCopySourceSseCustomerAlgorithm, request.SseCustomerAlgorithm);
            request.SetHeader(AmzHeaders.XAmzCopySourceSseCustomerKey, request.SseCustomerKey, BinaryEncoding.Base64);
            request.SetHeader(AmzHeaders.XAmzCopySourceSseCustomerKeyMd5, request.SseCustomerKeyMd5, BinaryEncoding.Base64);

            request.SetHeader(AmzHeaders.XAmzMetadataDirective, request.MetadataDirective);
            request.SetHeader(AmzHeaders.XAmzTaggingDirective, request.TaggingDirective);
            return null;
        }
    }
}