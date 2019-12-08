using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Core.Abstracts.Features;
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
            Func<Type, bool> disabledFor = x => false;

            if (req is IAutoMapConfig autoMap)
                disabledFor = autoMap.AutoMapDisabledFor;

            if (req is IHasBucketAcl hasBucketAcl && !disabledFor(typeof(IHasBucketAcl)))
            {
                req.AddHeader(AmzHeaders.XAmzAcl, hasBucketAcl.Acl);
                req.AddHeader(AmzHeaders.XAmzGrantRead, hasBucketAcl.AclGrantRead);
                req.AddHeader(AmzHeaders.XAmzGrantReadAcp, hasBucketAcl.AclGrantReadAcp);
                req.AddHeader(AmzHeaders.XAmzGrantWrite, hasBucketAcl.AclGrantWrite);
                req.AddHeader(AmzHeaders.XAmzGrantWriteAcp, hasBucketAcl.AclGrantWriteAcp);
                req.AddHeader(AmzHeaders.XAmzGrantFullControl, hasBucketAcl.AclGrantFullControl);
            }

            if (req is IHasBypassGovernanceRetention hasBypassGovernanceRetention && !disabledFor(typeof(IHasBypassGovernanceRetention)))
                req.AddHeader(AmzHeaders.XAmzBypassGovernanceRetention, hasBypassGovernanceRetention.BypassGovernanceRetention);

            if (req is IHasCache hasCache && !disabledFor(typeof(IHasCache)))
            {
                req.AddHeader(HttpHeaders.IfMatch, hasCache.IfETagMatch);
                req.AddHeader(HttpHeaders.IfNoneMatch, hasCache.IfETagNotMatch);
                req.AddHeader(HttpHeaders.IfModifiedSince, hasCache.IfModifiedSince, DateTimeFormat.Rfc1123);
                req.AddHeader(HttpHeaders.IfUnmodifiedSince, hasCache.IfUnmodifiedSince, DateTimeFormat.Rfc1123);
            }

            if (req is IHasCacheControl hasCacheControl && !disabledFor(typeof(IHasCacheControl)))
                req.AddHeader(HttpHeaders.CacheControl, hasCacheControl.CacheControl);

            if (req is IHasContentProps hasContentProps && !disabledFor(typeof(IHasContentProps)))
            {
                req.AddHeader(HttpHeaders.ContentDisposition, hasContentProps.ContentDisposition);
                req.AddHeader(HttpHeaders.ContentEncoding, hasContentProps.ContentEncoding);
                req.AddHeader(HttpHeaders.ContentType, hasContentProps.ContentType);
            }

            if (req is IHasExpiresOn hasExpiresOn && !disabledFor(typeof(IHasExpiresOn)))
                req.AddHeader(HttpHeaders.Expires, hasExpiresOn.ExpiresOn, DateTimeFormat.Rfc1123);

            if (req is IHasLock hasLock && !disabledFor(typeof(IHasLock)))
            {
                req.AddHeader(AmzHeaders.XAmzObjectLockMode, hasLock.LockMode);
                req.AddHeader(AmzHeaders.XAmzObjectLockRetainUntilDate, hasLock.LockRetainUntil, DateTimeFormat.Iso8601DateTimeExt);
            }

            if (req is IHasLegalHold hasLegalHold && !disabledFor(typeof(IHasLegalHold)))
                req.AddHeader(AmzHeaders.XAmzObjectLockLegalHold, hasLegalHold.LockLegalHold);

            if (req is IHasMetadata hasMetadata && !disabledFor(typeof(IHasMetadata)))
            {
                if (hasMetadata.Metadata != null)
                {
                    foreach (KeyValuePair<string, string> item in hasMetadata.Metadata.GetPrefixed())
                        req.AddHeader(item.Key, item.Value);
                }
            }

            if (req is IHasMfa hasMfa && !disabledFor(typeof(IHasMfa)))
                req.AddHeader(AmzHeaders.XAmzMfa, hasMfa.Mfa);

            if (req is IHasObjectAcl hasAcl && !disabledFor(typeof(IHasObjectAcl)))
            {
                req.AddHeader(AmzHeaders.XAmzAcl, hasAcl.Acl);
                req.AddHeader(AmzHeaders.XAmzGrantRead, hasAcl.AclGrantRead);
                req.AddHeader(AmzHeaders.XAmzGrantReadAcp, hasAcl.AclGrantReadAcp);
                req.AddHeader(AmzHeaders.XAmzGrantWriteAcp, hasAcl.AclGrantWriteAcp);
                req.AddHeader(AmzHeaders.XAmzGrantFullControl, hasAcl.AclGrantFullControl);
            }

            if (req is IHasPartNumber hasPartNumber && !disabledFor(typeof(IHasPartNumber)))
                req.AddQueryParameter(AmzParameters.PartNumber, hasPartNumber.PartNumber);

            if (req is IHasRange hasRange && !disabledFor(typeof(IHasRange)))
                req.AddHeader(HttpHeaders.Range, hasRange.Range);

            if (req is IHasRequestPayer hasRequestPayer && !disabledFor(typeof(IHasRequestPayer)))
                req.AddHeader(AmzHeaders.XAmzRequestPayer, hasRequestPayer.RequestPayer == Payer.Requester ? "requester" : null);

            if (req is IHasResponseHeader hasResponseHeader && !disabledFor(typeof(IHasResponseHeader)))
            {
                req.AddQueryParameter(AmzParameters.ResponseCacheControl, hasResponseHeader.ResponseCacheControl);
                req.AddQueryParameter(AmzParameters.ResponseExpires, hasResponseHeader.ResponseExpires, DateTimeFormat.Rfc1123);
                req.AddQueryParameter(AmzParameters.ResponseContentDisposition, hasResponseHeader.ResponseContentDisposition);
                req.AddQueryParameter(AmzParameters.ResponseContentEncoding, hasResponseHeader.ResponseContentEncoding);
                req.AddQueryParameter(AmzParameters.ResponseContentLanguage, hasResponseHeader.ResponseContentLanguage);
                req.AddQueryParameter(AmzParameters.ResponseContentType, hasResponseHeader.ResponseContentType);
            }

            if (req is IHasSse hasSse && !disabledFor(typeof(IHasSse)))
            {
                req.AddHeader(AmzHeaders.XAmzSse, hasSse.SseAlgorithm);
                req.AddHeader(AmzHeaders.XAmzSseAwsKmsKeyId, hasSse.SseKmsKeyId);

                string sseContext = hasSse.SseContext.Build();

                if (sseContext != null)
                    req.AddHeader(AmzHeaders.XAmzSseContext, Encoding.UTF8.GetBytes(sseContext), BinaryEncoding.Base64);
            }

            if (req is IHasSseCustomerKey hasSseCustomerKey && !disabledFor(typeof(IHasSseCustomerKey)))
            {
                req.AddHeader(AmzHeaders.XAmzSseCustomerAlgorithm, hasSseCustomerKey.SseCustomerAlgorithm);
                req.AddHeader(AmzHeaders.XAmzSseCustomerKey, hasSseCustomerKey.SseCustomerKey, BinaryEncoding.Base64);
                req.AddHeader(AmzHeaders.XAmzSseCustomerKeyMd5, hasSseCustomerKey.SseCustomerKeyMd5, BinaryEncoding.Base64);
            }

            if (req is IHasStorageClass hasStorageClass && !disabledFor(typeof(IHasStorageClass)))
                req.AddHeader(AmzHeaders.XAmzStorageClass, hasStorageClass.StorageClass);

            if (req is IHasTags hasTags && !disabledFor(typeof(IHasTags)))
                req.AddHeader(AmzHeaders.XAmzTagging, hasTags.Tags);

            if (req is IHasUploadId hasUploadId && !disabledFor(typeof(IHasUploadId)))
                req.AddQueryParameter(AmzParameters.UploadId, hasUploadId.UploadId);

            if (req is IHasVersionId hasVersionId && !disabledFor(typeof(IHasVersionId)))
                req.AddQueryParameter(AmzParameters.VersionId, hasVersionId.VersionId);

            if (req is IHasWebsiteRedirect hasWebsiteRedirect && !disabledFor(typeof(IHasWebsiteRedirect)))
                req.AddHeader(AmzHeaders.XAmzWebsiteRedirectLocation, hasWebsiteRedirect.WebsiteRedirectLocation);
        }
    }
}