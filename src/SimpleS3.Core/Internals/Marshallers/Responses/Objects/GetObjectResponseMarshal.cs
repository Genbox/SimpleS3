using System;
using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Marshallers;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects
{
    [UsedImplicitly]
    internal class GetObjectResponseMarshal : IResponseMarshal<GetObjectRequest, GetObjectResponse>
    {
        public void MarshalResponse(IConfig config, GetObjectRequest request, GetObjectResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.Metadata = HeaderParserHelper.ParseMetadata(headers);
            response.ETag = headers.GetHeader(HttpHeaders.ETag);
            response.CacheControl = headers.GetHeader(HttpHeaders.CacheControl);
            response.LastModified = headers.GetHeaderDate(HttpHeaders.LastModified, DateTimeFormat.Rfc1123);
            response.ContentType = headers.GetHeader(HttpHeaders.ContentType);
            response.ContentDisposition = headers.GetHeader(HttpHeaders.ContentDisposition);
            response.ContentEncoding = headers.GetHeader(HttpHeaders.ContentEncoding);
            response.ContentLanguage = headers.GetHeader(HttpHeaders.ContentLanguage);
            response.ContentRange = headers.GetHeader(HttpHeaders.ContentRange);
            response.AcceptRanges = headers.GetHeader(HttpHeaders.AcceptRanges);
            response.ExpiresOn = headers.GetHeaderDate(HttpHeaders.Expires, DateTimeFormat.Rfc1123);
            response.ReplicationStatus = headers.GetHeaderEnum<ReplicationStatus>(AmzHeaders.XAmzReplicationStatus);
            response.SseAlgorithm = headers.GetHeaderEnum<SseAlgorithm>(AmzHeaders.XAmzSse);
            response.SseKmsKeyId = headers.GetHeader(AmzHeaders.XAmzSseAwsKmsKeyId);
            response.SseCustomerAlgorithm = headers.GetHeaderEnum<SseCustomerAlgorithm>(AmzHeaders.XAmzSseCustomerAlgorithm);
            response.SseCustomerKeyMd5 = headers.GetHeaderByteArray(AmzHeaders.XAmzSseCustomerKeyMd5, BinaryEncoding.Base64);
            response.IsDeleteMarker = headers.GetHeaderBool(AmzHeaders.XAmzDeleteMarker);
            response.VersionId = headers.GetHeader(AmzHeaders.XAmzVersionId);
            response.RequestCharged = headers.ContainsKey(AmzHeaders.XAmzRequestCharged);

            response.StorageClass = headers.GetHeaderEnum<StorageClass>(AmzHeaders.XAmzStorageClass);

            //It should default to standard
            if (response.StorageClass == StorageClass.Unknown)
                response.StorageClass = StorageClass.Standard;

            response.Restore = headers.GetHeader(AmzHeaders.XAmzRestore);
            response.TagCount = headers.GetHeaderInt(AmzHeaders.XAmzTaggingCount);
            response.WebsiteRedirectLocation = headers.GetHeader(AmzHeaders.XAmzWebsiteRedirectLocation);
            response.LockMode = headers.GetHeaderEnum<LockMode>(AmzHeaders.XAmzObjectLockMode);
            response.LockRetainUntil = headers.GetHeaderDate(AmzHeaders.XAmzObjectLockRetainUntilDate, DateTimeFormat.Iso8601DateTimeExt);
            response.LockLegalHold = headers.GetHeader(AmzHeaders.XAmzObjectLockLegalHold) == "ON";
            response.NumberOfParts = headers.GetHeaderInt(AmzHeaders.XAmzPartsCount);
            response.Content = new ContentReader(responseStream);

            if (HeaderParserHelper.TryParseExpiration(headers, out (DateTimeOffset expiresOn, string ruleId) data))
            {
                response.LifeCycleExpiresOn = data.expiresOn;
                response.LifeCycleRuleId = data.ruleId;
            }
        }
    }
}