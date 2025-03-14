using System.Text;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Enums;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Internals.Marshallers.Requests;

internal static class GenericRequestMapper
{
    public static void Map<T>(T req) where T : IRequest
    {
        Func<Type, bool> disabledFor = _ => false;

        if (req is IAutoMapConfig autoMap)
            disabledFor = autoMap.AutoMapDisabledFor;

        if (req is IHasBucketAcl hasBucketAcl && !disabledFor(typeof(IHasBucketAcl)))
        {
            if (hasBucketAcl.Acl != BucketCannedAcl.Unknown)
                req.SetHeader(AmzHeaders.XAmzAcl, hasBucketAcl.Acl.GetDisplayName());

            req.SetHeader(AmzHeaders.XAmzGrantRead, hasBucketAcl.AclGrantRead);
            req.SetHeader(AmzHeaders.XAmzGrantReadAcp, hasBucketAcl.AclGrantReadAcp);
            req.SetHeader(AmzHeaders.XAmzGrantWrite, hasBucketAcl.AclGrantWrite);
            req.SetHeader(AmzHeaders.XAmzGrantWriteAcp, hasBucketAcl.AclGrantWriteAcp);
            req.SetHeader(AmzHeaders.XAmzGrantFullControl, hasBucketAcl.AclGrantFullControl);

            if (hasBucketAcl.ObjectOwnership != ObjectOwnership.Unknown)
                req.SetHeader(AmzHeaders.XAmzObjectOwnership, hasBucketAcl.ObjectOwnership.GetString());
        }

        if (req is IHasBypassGovernanceRetention hasBypassGovernanceRetention && !disabledFor(typeof(IHasBypassGovernanceRetention)))
            req.SetHeader(AmzHeaders.XAmzBypassGovernanceRetention, hasBypassGovernanceRetention.BypassGovernanceRetention);

        if (req is IHasCache hasCache && !disabledFor(typeof(IHasCache)))
        {
            req.SetHeader(HttpHeaders.IfMatch, hasCache.IfETagMatch);
            req.SetHeader(HttpHeaders.IfNoneMatch, hasCache.IfETagNotMatch);
            req.SetHeader(HttpHeaders.IfModifiedSince, hasCache.IfModifiedSince, DateTimeFormat.Rfc1123);
            req.SetHeader(HttpHeaders.IfUnmodifiedSince, hasCache.IfUnmodifiedSince, DateTimeFormat.Rfc1123);
        }

        if (req is IHasCacheControl hasCacheControl && !disabledFor(typeof(IHasCacheControl)))
            req.SetHeader(HttpHeaders.CacheControl, hasCacheControl.CacheControl);

        if (req is IHasContentProps hasContentProps && !disabledFor(typeof(IHasContentProps)))
        {
            req.SetHeader(HttpHeaders.ContentDisposition, hasContentProps.ContentDisposition);
            req.SetHeader(HttpHeaders.ContentEncoding, hasContentProps.ContentEncoding);
            req.SetHeader(HttpHeaders.ContentType, hasContentProps.ContentType);
        }

        if (req is IHasExpiresOn hasExpiresOn && !disabledFor(typeof(IHasExpiresOn)))
            req.SetHeader(HttpHeaders.Expires, hasExpiresOn.ExpiresOn, DateTimeFormat.Rfc1123);

        if (req is IHasLock hasLock && !disabledFor(typeof(IHasLock)) && hasLock.LockMode != LockMode.Unknown)
        {
            req.SetHeader(AmzHeaders.XAmzObjectLockMode, hasLock.LockMode.GetDisplayName());
            req.SetHeader(AmzHeaders.XAmzObjectLockRetainUntilDate, hasLock.LockRetainUntil, DateTimeFormat.Iso8601DateTimeExt);
        }

        if (req is IHasLegalHold hasLegalHold && !disabledFor(typeof(IHasLegalHold)))
            req.SetHeader(AmzHeaders.XAmzObjectLockLegalHold, hasLegalHold.LockLegalHold);

        if (req is IHasMetadata hasMetadata && !disabledFor(typeof(IHasMetadata)))
        {
            foreach (KeyValuePair<string, string> item in hasMetadata.Metadata.GetPrefixed())
                req.SetHeader(item.Key, item.Value);
        }

        if (req is IHasMfa hasMfa && !disabledFor(typeof(IHasMfa)))
            req.SetHeader(AmzHeaders.XAmzMfa, hasMfa.Mfa);

        if (req is IHasObjectAcl hasAcl && !disabledFor(typeof(IHasObjectAcl)))
        {
            if (hasAcl.Acl != ObjectCannedAcl.Unknown)
                req.SetHeader(AmzHeaders.XAmzAcl, hasAcl.Acl.GetDisplayName());

            req.SetHeader(AmzHeaders.XAmzGrantRead, hasAcl.AclGrantRead);
            req.SetHeader(AmzHeaders.XAmzGrantReadAcp, hasAcl.AclGrantReadAcp);
            req.SetHeader(AmzHeaders.XAmzGrantWriteAcp, hasAcl.AclGrantWriteAcp);
            req.SetHeader(AmzHeaders.XAmzGrantFullControl, hasAcl.AclGrantFullControl);
        }

        if (req is IHasPartNumber hasPartNumber && !disabledFor(typeof(IHasPartNumber)))
            req.SetQueryParameter(AmzParameters.PartNumber, hasPartNumber.PartNumber);

        if (req is IHasRange hasRange && !disabledFor(typeof(IHasRange)))
            req.SetHeader(HttpHeaders.Range, hasRange.Range);

        if (req is IHasRequestPayer hasRequestPayer && !disabledFor(typeof(IHasRequestPayer)))
            req.SetOptionalHeader(AmzHeaders.XAmzRequestPayer, hasRequestPayer.RequestPayer == Payer.Requester ? "requester" : null);

        if (req is IHasResponseHeader hasResponseHeader && !disabledFor(typeof(IHasResponseHeader)))
        {
            req.SetQueryParameter(AmzParameters.ResponseCacheControl, hasResponseHeader.ResponseCacheControl);
            req.SetQueryParameter(AmzParameters.ResponseExpires, hasResponseHeader.ResponseExpires, DateTimeFormat.Rfc1123);
            req.SetQueryParameter(AmzParameters.ResponseContentDisposition, hasResponseHeader.ResponseContentDisposition);
            req.SetQueryParameter(AmzParameters.ResponseContentEncoding, hasResponseHeader.ResponseContentEncoding);
            req.SetQueryParameter(AmzParameters.ResponseContentLanguage, hasResponseHeader.ResponseContentLanguage);
            req.SetQueryParameter(AmzParameters.ResponseContentType, hasResponseHeader.ResponseContentType);
        }

        if (req is IHasSse hasSse && !disabledFor(typeof(IHasSse)))
        {
            if (hasSse.SseAlgorithm != SseAlgorithm.Unknown)
                req.SetHeader(AmzHeaders.XAmzSse, hasSse.SseAlgorithm.GetDisplayName());

            req.SetOptionalHeader(AmzHeaders.XAmzSseAwsKmsKeyId, hasSse.SseKmsKeyId);

            string? sseContext = hasSse.SseContext.Build();

            if (sseContext != null)
                req.SetHeader(AmzHeaders.XAmzSseContext, Encoding.UTF8.GetBytes(sseContext), BinaryEncoding.Base64);
        }

        if (req is IHasSseCustomerKey hasSseCustomerKey && !disabledFor(typeof(IHasSseCustomerKey)))
        {
            if (hasSseCustomerKey.SseCustomerAlgorithm != SseCustomerAlgorithm.Unknown)
                req.SetHeader(AmzHeaders.XAmzSseCustomerAlgorithm, hasSseCustomerKey.SseCustomerAlgorithm.GetDisplayName());

            req.SetHeader(AmzHeaders.XAmzSseCustomerKey, hasSseCustomerKey.SseCustomerKey, BinaryEncoding.Base64);
            req.SetHeader(AmzHeaders.XAmzSseCustomerKeyMd5, hasSseCustomerKey.SseCustomerKeyMd5, BinaryEncoding.Base64);
        }

        if (req is IHasStorageClass hasStorageClass && !disabledFor(typeof(IHasStorageClass)) && hasStorageClass.StorageClass != StorageClass.Unknown)
            req.SetHeader(AmzHeaders.XAmzStorageClass, hasStorageClass.StorageClass.GetDisplayName());

        if (req is IHasTags hasTags && !disabledFor(typeof(IHasTags)))
            req.SetHeader(AmzHeaders.XAmzTagging, hasTags.Tags);

        if (req is IHasUploadId hasUploadId && !disabledFor(typeof(IHasUploadId)))
            req.SetQueryParameter(AmzParameters.UploadId, hasUploadId.UploadId);

        if (req is IHasVersionId hasVersionId && !disabledFor(typeof(IHasVersionId)))
            req.SetOptionalQueryParameter(AmzParameters.VersionId, hasVersionId.VersionId);

        if (req is IHasWebsiteRedirect hasWebsiteRedirect && !disabledFor(typeof(IHasWebsiteRedirect)))
            req.SetOptionalHeader(AmzHeaders.XAmzWebsiteRedirectLocation, hasWebsiteRedirect.WebsiteRedirectLocation);

        if (req is IHasExpectedBucketOwner expectedBucketOwner && expectedBucketOwner.ExpectedBucketOwner != null)
            req.SetHeader(AmzHeaders.XAmzExpectedBucketOwner, expectedBucketOwner.ExpectedBucketOwner);

        if (req is IHasChecksum checksum)
        {
            if (checksum.ChecksumType != ChecksumType.Unknown)
                req.SetHeader(AmzHeaders.XAmzChecksumType, checksum.ChecksumType.GetDisplayName());

            if (checksum.ChecksumAlgorithm != ChecksumAlgorithm.Unknown)
                req.SetHeader(AmzHeaders.XAmzChecksumAlgorithm, checksum.ChecksumAlgorithm.GetDisplayName());
        }

        if (req is IHasChecksumProperties checksumProps && checksumProps.ChecksumAlgorithm != ChecksumAlgorithm.Unknown && checksumProps.Checksum != null)
            req.SetHeader(AmzHeaders.XAmzChecksum + checksumProps.ChecksumAlgorithm.ToString().ToLowerInvariant(), Convert.ToBase64String(checksumProps.Checksum));
    }
}