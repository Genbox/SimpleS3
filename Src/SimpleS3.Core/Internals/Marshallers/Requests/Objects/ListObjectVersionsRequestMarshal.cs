using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests.Objects;

internal sealed class ListObjectVersionsRequestMarshal : IRequestMarshal<ListObjectVersionsRequest>
{
    public Stream? MarshalRequest(ListObjectVersionsRequest request, SimpleS3Config config)
    {
        request.SetQueryParameter(AmzParameters.Versions, string.Empty);

        // For ?versions, GCS defaults to legacy XML tags (Generation/NextGenerationMarker) instead of S3's VersionId/NextVersionIdMarker.
        // Opt into its interoperable response format so the shared S3 response marshaller can read version IDs and pagination markers.
        // Use the x-amz alias with AWS4 signing: GCS rejects a request that mixes x-goog and x-amz headers.
        // https://docs.cloud.google.com/storage/docs/xml-api/reference-headers#xgooginteroplistobjectsformat
        // https://docs.cloud.google.com/storage/docs/xml-api/reference-headers#versions
        // https://docs.cloud.google.com/storage/docs/aws-simple-migration#authheader
        // https://issuetracker.google.com/issues/312292516
        if (string.Equals(config.ProviderName, "GoogleCloudStorage", StringComparison.OrdinalIgnoreCase))
            request.SetHeader("x-amz-interop-list-objects-format", "enabled");

        request.SetOptionalQueryParameter(AmzParameters.Delimiter, request.Delimiter);

        if (request.EncodingType != EncodingType.Unknown)
            request.SetQueryParameter(AmzParameters.EncodingType, request.EncodingType.GetDisplayName());

        request.SetOptionalQueryParameter(AmzParameters.KeyMarker, request.KeyMarker);
        request.SetQueryParameter(AmzParameters.MaxKeys, request.MaxKeys);
        request.SetOptionalQueryParameter(AmzParameters.Prefix, request.Prefix);
        request.SetOptionalQueryParameter(AmzParameters.VersionIdMarker, request.VersionIdMarker);

        string optionalAttributes = string.Join(",", request.OptionalObjectAttributes.Where(x => x != OptionalObjectAttribute.Unknown).Select(x => x.GetDisplayName()));
        if (optionalAttributes.Length > 0)
            request.SetHeader(AmzHeaders.XAmzOptionalObjectAttributes, optionalAttributes);

        return null;
    }
}