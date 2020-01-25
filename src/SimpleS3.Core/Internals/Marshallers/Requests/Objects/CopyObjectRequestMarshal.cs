using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Marshallers;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects
{
    [UsedImplicitly]
    internal class CopyObjectRequestMarshal : IRequestMarshal<CopyObjectRequest>
    {
        public Stream MarshalRequest(CopyObjectRequest request, IConfig config)
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