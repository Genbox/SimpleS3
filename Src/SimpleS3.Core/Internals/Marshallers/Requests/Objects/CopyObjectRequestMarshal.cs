using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects;

internal sealed class CopyObjectRequestMarshal : IRequestMarshal<CopyObjectRequest>
{
    public Stream? MarshalRequest(CopyObjectRequest request, SimpleS3Config config)
    {
        request.SetHeader(AmzHeaders.XAmzCopySource, '/' + request.SourceBucketName + '/' + request.SourceObjectKey);
        request.SetHeader(AmzHeaders.XAmzCopySourceIfMatch, request.IfETagMatch);
        request.SetHeader(AmzHeaders.XAmzCopySourceIfNoneMatch, request.IfETagNotMatch);
        request.SetHeader(AmzHeaders.XAmzCopySourceIfModifiedSince, request.IfModifiedSince, DateTimeFormat.Rfc1123);
        request.SetHeader(AmzHeaders.XAmzCopySourceIfUnmodifiedSince, request.IfUnmodifiedSince, DateTimeFormat.Rfc1123);

        if (request.SseCustomerAlgorithm != SseCustomerAlgorithm.Unknown)
            request.SetHeader(AmzHeaders.XAmzCopySourceSseCustomerAlgorithm, request.SseCustomerAlgorithm.GetDisplayName());

        request.SetHeader(AmzHeaders.XAmzCopySourceSseCustomerKey, request.SseCustomerKey, BinaryEncoding.Base64);
        request.SetHeader(AmzHeaders.XAmzCopySourceSseCustomerKeyMd5, request.SseCustomerKeyMd5, BinaryEncoding.Base64);

        if (request.MetadataDirective != MetadataDirective.Unknown)
            request.SetHeader(AmzHeaders.XAmzMetadataDirective, request.MetadataDirective.GetDisplayName());

        if (request.TaggingDirective != TaggingDirective.Unknown)
            request.SetHeader(AmzHeaders.XAmzTaggingDirective, request.TaggingDirective.GetDisplayName());
        return null;
    }
}