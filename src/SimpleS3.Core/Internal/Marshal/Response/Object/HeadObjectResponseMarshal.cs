using System;
using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Response.Object
{
    [UsedImplicitly]
    internal class HeadObjectResponseMarshal : IResponseMarshal<HeadObjectRequest, HeadObjectResponse>
    {
        public void MarshalResponse(IS3Config config, HeadObjectRequest request, HeadObjectResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.ExpiresOn = headers.GetHeader(AmzHeaders.XAmzExpiration);
            response.Metadata = ParseMetadata(headers);
            response.ETag = headers.GetHeader(HttpHeaders.ETag);
            response.CacheControl = headers.GetHeader(HttpHeaders.CacheControl);
            response.LastModified = headers.GetHeaderDate(HttpHeaders.LastModified, DateTimeFormat.Rfc1123);
            response.ContentType = headers.GetHeader(HttpHeaders.ContentType);
            response.ContentDisposition = headers.GetHeader(HttpHeaders.ContentDisposition);
            response.ContentEncoding = headers.GetHeader(HttpHeaders.ContentEncoding);
            response.ContentLanguage = headers.GetHeader(HttpHeaders.ContentLanguage);
            response.Expires = headers.GetHeaderDate(HttpHeaders.Expires, DateTimeFormat.Rfc1123);
            response.ReplicationStatus = headers.GetHeaderEnum<ReplicationStatus>(AmzHeaders.XAmzReplicationStatus);
            response.SseAlgorithm = headers.GetHeaderEnum<SseAlgorithm>(AmzHeaders.XAmzSSE);
            response.SseKmsKeyId = headers.GetHeader(AmzHeaders.XAmzSSEAwsKmsKeyId);
            response.SseCustomerAlgorithm = headers.GetHeaderEnum<SseCustomerAlgorithm>(AmzHeaders.XAmzSSECustomerAlgorithm);
            response.SseCustomerKeyMd5 = headers.GetHeaderByteArray(AmzHeaders.XAmzSSECustomerKeyMD5, BinaryEncoding.Base64);
            response.RequestCharged = string.Equals("RequestPayer", headers.GetHeader(AmzHeaders.XAmzVersionId), StringComparison.OrdinalIgnoreCase);

            response.StorageClass = headers.GetHeaderEnum<StorageClass>(AmzHeaders.XAmzStorageClass);

            //It should default to standard
            if (response.StorageClass == StorageClass.Unknown)
                response.StorageClass = StorageClass.Standard;

            response.Restore = headers.GetHeader(AmzHeaders.XAmzRestore);
            response.TagCount = headers.GetHeaderInt(AmzHeaders.XAmzTaggingCount);
            response.WebsiteRedirectLocation = headers.GetHeader(AmzHeaders.XAmzWebsiteRedirectLocation);
            response.LockMode = headers.GetHeaderEnum<LockMode>(AmzHeaders.XAmzObjectLockMode);
            response.LockRetainUntilDate = headers.GetHeaderDate(AmzHeaders.XAmzObjectLockRetainUntilDate, DateTimeFormat.Iso8601DateTimeExt);
            response.LockLegalHold = headers.GetHeaderBool(AmzHeaders.XAmzObjectLockLegalHold);
            response.NumberOfParts = headers.GetHeaderInt(AmzHeaders.XAmzPartsCount);
            response.VersionId = headers.GetHeader(AmzHeaders.XAmzVersionId);
        }

        private static IDictionary<string, string> ParseMetadata(IDictionary<string, string> headers)
        {
            string _metadataHeader = "x-amz-meta-";

            IDictionary<string, string> metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (KeyValuePair<string, string> item in headers)
            {
                if (!item.Key.StartsWith(_metadataHeader, StringComparison.OrdinalIgnoreCase))
                    continue;

                //If we crash here, it is because AWS sent us an invalid header.
                metadata[item.Key.Substring(_metadataHeader.Length)] = item.Value;
            }

            return metadata;
        }
    }
}