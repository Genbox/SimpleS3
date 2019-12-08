using System.Collections.Generic;
using System.Text;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Properties;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request
{
    internal static class GenericRequestMapper
    {
        public static void Map<T>(T req) where T : IRequest
        {
            if (req is IHasBucketAcl hasBucketAcl)
            {
                req.AddHeader(AmzHeaders.XAmzAcl, hasBucketAcl.Acl);
                req.AddHeader(AmzHeaders.XAmzGrantRead, hasBucketAcl.AclGrantRead);
                req.AddHeader(AmzHeaders.XAmzGrantReadAcp, hasBucketAcl.AclGrantReadAcp);
                req.AddHeader(AmzHeaders.XAmzGrantWrite, hasBucketAcl.AclGrantWrite);
                req.AddHeader(AmzHeaders.XAmzGrantWriteAcp, hasBucketAcl.AclGrantWriteAcp);
                req.AddHeader(AmzHeaders.XAmzGrantFullControl, hasBucketAcl.AclGrantFullControl);
            }

            if (req is IHasBypassGovernanceRetention hasBypassGovernanceRetention)
                req.AddHeader(AmzHeaders.XAmzBypassGovernanceRetention, hasBypassGovernanceRetention.BypassGovernanceRetention);

            if (req is IHasCache hasCache)
            {
                req.AddHeader(HttpHeaders.IfMatch, hasCache.IfETagMatch);
                req.AddHeader(HttpHeaders.IfNoneMatch, hasCache.IfETagNotMatch);
                req.AddHeader(HttpHeaders.IfModifiedSince, hasCache.IfModifiedSince, DateTimeFormat.Rfc1123);
                req.AddHeader(HttpHeaders.IfUnmodifiedSince, hasCache.IfUnmodifiedSince, DateTimeFormat.Rfc1123);
            }

            if (req is IHasCacheControl hasCacheControl)
                req.AddHeader(HttpHeaders.CacheControl, hasCacheControl.CacheControl);

            if (req is IHasContentProps hasContentProps)
            {
                req.AddHeader(HttpHeaders.ContentDisposition, hasContentProps.ContentDisposition);
                req.AddHeader(HttpHeaders.ContentEncoding, hasContentProps.ContentEncoding);
                req.AddHeader(HttpHeaders.ContentType, hasContentProps.ContentType);
            }

            if (req is IHasExpiresOn hasExpiresOn)
                req.AddHeader(HttpHeaders.Expires, hasExpiresOn.ExpiresOn, DateTimeFormat.Rfc1123);

            if (req is IHasLock hasLock)
            {
                req.AddHeader(AmzHeaders.XAmzObjectLockMode, hasLock.LockMode);
                req.AddHeader(AmzHeaders.XAmzObjectLockRetainUntilDate, hasLock.LockRetainUntil, DateTimeFormat.Iso8601DateTimeExt);
            }

            if (req is IHasLegalHold hasLegalHold)
                req.AddHeader(AmzHeaders.XAmzObjectLockLegalHold, hasLegalHold.LockLegalHold);

            if (req is IHasMetadata hasMetadata)
            {
                if (hasMetadata.Metadata != null)
                {
                    foreach (KeyValuePair<string, string> item in hasMetadata.Metadata.GetPrefixed())
                        req.AddHeader(item.Key, item.Value);
                }
            }

            if (req is IHasMfa hasMfa)
                req.AddHeader(AmzHeaders.XAmzMfa, hasMfa.Mfa);

            if (req is IHasObjectAcl hasAcl)
            {
                req.AddHeader(AmzHeaders.XAmzAcl, hasAcl.Acl);
                req.AddHeader(AmzHeaders.XAmzGrantRead, hasAcl.AclGrantRead);
                req.AddHeader(AmzHeaders.XAmzGrantReadAcp, hasAcl.AclGrantReadAcp);
                req.AddHeader(AmzHeaders.XAmzGrantWriteAcp, hasAcl.AclGrantWriteAcp);
                req.AddHeader(AmzHeaders.XAmzGrantFullControl, hasAcl.AclGrantFullControl);
            }

            if (req is IHasPartNumber hasPartNumber)
                req.AddQueryParameter(AmzParameters.PartNumber, hasPartNumber.PartNumber);

            if (req is IHasRange hasRange)
                req.AddHeader(HttpHeaders.Range, hasRange.Range);

            if (req is IHasRequestPayer hasRequestPayer)
                req.AddHeader(AmzHeaders.XAmzRequestPayer, hasRequestPayer.RequestPayer == Payer.Requester ? "requester" : null);

            if (req is IHasResponseHeader hasResponseHeader)
            {
                req.AddQueryParameter(AmzParameters.ResponseCacheControl, hasResponseHeader.ResponseCacheControl);
                req.AddQueryParameter(AmzParameters.ResponseExpires, hasResponseHeader.ResponseExpires, DateTimeFormat.Rfc1123);
                req.AddQueryParameter(AmzParameters.ResponseContentDisposition, hasResponseHeader.ResponseContentDisposition);
                req.AddQueryParameter(AmzParameters.ResponseContentEncoding, hasResponseHeader.ResponseContentEncoding);
                req.AddQueryParameter(AmzParameters.ResponseContentLanguage, hasResponseHeader.ResponseContentLanguage);
                req.AddQueryParameter(AmzParameters.ResponseContentType, hasResponseHeader.ResponseContentType);
            }

            if (req is IHasSse hasSse)
            {
                req.AddHeader(AmzHeaders.XAmzSse, hasSse.SseAlgorithm);
                req.AddHeader(AmzHeaders.XAmzSseAwsKmsKeyId, hasSse.SseKmsKeyId);

                string sseContext = hasSse.SseContext.Build();

                if (sseContext != null)
                    req.AddHeader(AmzHeaders.XAmzSseContext, Encoding.UTF8.GetBytes(sseContext), BinaryEncoding.Base64);
            }

            if (req is IHasSseCustomerKey hasSseCustomerKey)
            {
                req.AddHeader(AmzHeaders.XAmzSseCustomerAlgorithm, hasSseCustomerKey.SseCustomerAlgorithm);
                req.AddHeader(AmzHeaders.XAmzSseCustomerKey, hasSseCustomerKey.SseCustomerKey, BinaryEncoding.Base64);
                req.AddHeader(AmzHeaders.XAmzSseCustomerKeyMd5, hasSseCustomerKey.SseCustomerKeyMd5, BinaryEncoding.Base64);
            }

            if (req is IHasStorageClass hasStorageClass)
                req.AddHeader(AmzHeaders.XAmzStorageClass, hasStorageClass.StorageClass);

            if (req is IHasTags hasTags)
                req.AddHeader(AmzHeaders.XAmzTagging, hasTags.Tags);

            if (req is IHasUploadId hasUploadId)
                req.AddQueryParameter(AmzParameters.UploadId, hasUploadId.UploadId);

            if (req is IHasVersionId hasVersionId)
                req.AddQueryParameter(AmzParameters.VersionId, hasVersionId.VersionId);

            if (req is IHasWebsiteRedirect hasWebsiteRedirect)
                req.AddHeader(AmzHeaders.XAmzWebsiteRedirectLocation, hasWebsiteRedirect.WebsiteRedirectLocation);
        }
    }
}