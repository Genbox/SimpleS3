using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Helpers;
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
        string copySource = '/' + UrlHelper.UrlEncode(request.SourceBucketName) + '/' + UrlHelper.UrlPathEncode(request.SourceObjectKey);

        // S3-003: A copy version identifies the source and is part of x-amz-copy-source.
        if (request.VersionId != null)
            copySource += "?versionId=" + UrlHelper.UrlEncode(request.VersionId);

        request.SetHeader(AmzHeaders.XAmzCopySource, copySource);
        request.SetOptionalHeader(AmzHeaders.XAmzSourceExpectedBucketOwner, request.ExpectedSourceBucketOwner);
        request.SetHeader(AmzHeaders.XAmzCopySourceIfMatch, request.IfETagMatch);
        request.SetHeader(AmzHeaders.XAmzCopySourceIfNoneMatch, request.IfETagNotMatch);
        request.SetHeader(AmzHeaders.XAmzCopySourceIfModifiedSince, request.IfModifiedSince, DateTimeFormat.Rfc1123);
        request.SetHeader(AmzHeaders.XAmzCopySourceIfUnmodifiedSince, request.IfUnmodifiedSince, DateTimeFormat.Rfc1123);

        // Wasabi accepts failed source conditions and performs the copy, so never send a conditional copy there.
        if (string.Equals(config.ProviderName, "Wasabi", StringComparison.OrdinalIgnoreCase) && (request.Headers.ContainsKey(AmzHeaders.XAmzCopySourceIfMatch) || request.Headers.ContainsKey(AmzHeaders.XAmzCopySourceIfNoneMatch) || request.Headers.ContainsKey(AmzHeaders.XAmzCopySourceIfModifiedSince) || request.Headers.ContainsKey(AmzHeaders.XAmzCopySourceIfUnmodifiedSince)))
            throw new NotSupportedException("Wasabi does not enforce source conditions for CopyObject. The copy was not sent.");

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