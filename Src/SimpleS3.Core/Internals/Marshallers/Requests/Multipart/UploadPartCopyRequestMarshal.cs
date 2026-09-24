using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Multipart;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Multipart;

internal sealed class UploadPartCopyRequestMarshal : IRequestMarshal<UploadPartCopyRequest>
{
    public Stream? MarshalRequest(UploadPartCopyRequest request, SimpleS3Config config)
    {
        request.SetHeader(AmzHeaders.XAmzCopySource, '/' + UrlHelper.UrlEncode(request.SourceBucketName) + '/' + UrlHelper.UrlPathEncode(request.SourceObjectKey));
        request.SetOptionalHeader(AmzHeaders.XAmzSourceExpectedBucketOwner, request.ExpectedSourceBucketOwner);
        request.SetHeader(AmzHeaders.XAmzCopySourceIfMatch, request.CopySourceIfETagMatch);
        request.SetHeader(AmzHeaders.XAmzCopySourceIfNoneMatch, request.CopySourceIfETagNotMatch);
        request.SetHeader(AmzHeaders.XAmzCopySourceIfModifiedSince, request.CopySourceIfModifiedSince, DateTimeFormat.Rfc1123);
        request.SetHeader(AmzHeaders.XAmzCopySourceIfUnmodifiedSince, request.CopySourceIfUnmodifiedSince, DateTimeFormat.Rfc1123);

        // Wasabi returns HTTP 200 when these conditions fail, so reject the request rather than silently perform an unconditional copy.
        if (string.Equals(config.ProviderName, "Wasabi", StringComparison.OrdinalIgnoreCase) && (request.Headers.ContainsKey(AmzHeaders.XAmzCopySourceIfMatch) || request.Headers.ContainsKey(AmzHeaders.XAmzCopySourceIfNoneMatch) || request.Headers.ContainsKey(AmzHeaders.XAmzCopySourceIfModifiedSince) || request.Headers.ContainsKey(AmzHeaders.XAmzCopySourceIfUnmodifiedSince)))
            throw new NotSupportedException("Wasabi does not enforce source conditions for UploadPartCopy. The copy was not sent.");

        request.SetOptionalHeader(AmzHeaders.XAmzCopySourceRange, request.CopySourceRange);

        if (request.SourceSseCustomerAlgorithm != SseCustomerAlgorithm.Unknown)
            request.SetHeader(AmzHeaders.XAmzCopySourceSseCustomerAlgorithm, request.SourceSseCustomerAlgorithm.GetDisplayName());

        request.SetHeader(AmzHeaders.XAmzCopySourceSseCustomerKey, request.SourceSseCustomerKey, BinaryEncoding.Base64);
        request.SetHeader(AmzHeaders.XAmzCopySourceSseCustomerKeyMd5, request.SourceSseCustomerKeyMd5, BinaryEncoding.Base64);
        return null;
    }
}