using System;
using System.Collections.Generic;
using System.IO;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Response;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Responses.Objects
{
    [UsedImplicitly]
    internal class HeadObjectResponseMarshal : IResponseMarshal<HeadObjectResponse>
    {
        public void MarshalResponse(Config config, HeadObjectResponse response, IDictionary<string, string> headers, Stream responseStream)
        {
            response.Metadata = HeaderParserHelper.ParseMetadata(headers);
            response.ETag = headers.GetOptionalValue(HttpHeaders.ETag);
            response.CacheControl = headers.GetOptionalValue(HttpHeaders.CacheControl);
            response.LastModified = headers.GetHeaderDate(HttpHeaders.LastModified, DateTimeFormat.Rfc1123);
            response.ContentType = headers.GetOptionalValue(HttpHeaders.ContentType);
            response.ContentDisposition = headers.GetOptionalValue(HttpHeaders.ContentDisposition);
            response.ContentEncoding = headers.GetOptionalValue(HttpHeaders.ContentEncoding);
            response.ContentLanguage = headers.GetOptionalValue(HttpHeaders.ContentLanguage);
            response.ContentRange = headers.GetOptionalValue(HttpHeaders.ContentRange);
            response.AcceptRanges = headers.GetOptionalValue(HttpHeaders.AcceptRanges);
            response.ExpiresOn = headers.GetHeaderDate(HttpHeaders.Expires, DateTimeFormat.Rfc1123);
            response.ReplicationStatus = headers.GetHeaderEnum<ReplicationStatus>(AmzHeaders.XAmzReplicationStatus);
            response.SseAlgorithm = headers.GetHeaderEnum<SseAlgorithm>(AmzHeaders.XAmzSse);
            response.SseKmsKeyId = headers.GetOptionalValue(AmzHeaders.XAmzSseAwsKmsKeyId);
            response.SseCustomerAlgorithm = headers.GetHeaderEnum<SseCustomerAlgorithm>(AmzHeaders.XAmzSseCustomerAlgorithm);
            response.SseCustomerKeyMd5 = headers.GetHeaderByteArray(AmzHeaders.XAmzSseCustomerKeyMd5, BinaryEncoding.Base64);
            response.IsDeleteMarker = headers.GetHeaderBool(AmzHeaders.XAmzDeleteMarker);
            response.VersionId = headers.GetOptionalValue(AmzHeaders.XAmzVersionId);

            response.StorageClass = headers.GetHeaderEnum<StorageClass>(AmzHeaders.XAmzStorageClass);

            //It should default to standard
            if (response.StorageClass == StorageClass.Unknown)
                response.StorageClass = StorageClass.Standard;

            response.Restore = headers.GetOptionalValue(AmzHeaders.XAmzRestore);
            response.TagCount = headers.GetHeaderInt(AmzHeaders.XAmzTaggingCount);
            response.WebsiteRedirectLocation = headers.GetOptionalValue(AmzHeaders.XAmzWebsiteRedirectLocation);
            response.LockMode = headers.GetHeaderEnum<LockMode>(AmzHeaders.XAmzObjectLockMode);
            response.LockRetainUntil = headers.GetHeaderDate(AmzHeaders.XAmzObjectLockRetainUntilDate, DateTimeFormat.Iso8601DateTimeExt);
            response.LockLegalHold = headers.GetOptionalValue(AmzHeaders.XAmzObjectLockLegalHold) == "ON";
            response.NumberOfParts = headers.GetHeaderInt(AmzHeaders.XAmzPartsCount);

            if (HeaderParserHelper.TryParseExpiration(headers, out (DateTimeOffset expiresOn, string ruleId) data))
            {
                response.LifeCycleExpiresOn = data.expiresOn;
                response.LifeCycleRuleId = data.ruleId;
            }
        }
    }
}