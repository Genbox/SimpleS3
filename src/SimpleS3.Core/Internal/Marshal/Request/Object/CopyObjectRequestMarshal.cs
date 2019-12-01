using System.Collections.Generic;
using System.IO;
using System.Text;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request.Object
{
    [UsedImplicitly]
    internal class CopyObjectRequestMarshal : IRequestMarshal<CopyObjectRequest>
    {
        public Stream MarshalRequest(CopyObjectRequest request, IS3Config config)
        {
            request.AddQueryParameter(AmzParameters.VersionId, request.VersionId);
            request.AddHeader(AmzHeaders.XAmzRequestPayer, request.RequestPayer == Payer.Requester ? "requester" : null);
            request.AddHeader(AmzHeaders.XAmzAcl, request.Acl);
            request.AddHeader(AmzHeaders.XAmzCopySource, '/' + request.SourceBucketName + '/' + request.SourceObjectKey);
            request.AddHeader(AmzHeaders.XAmzCopySourceIfMatch, request.IfETagMatch);
            request.AddHeader(AmzHeaders.XAmzCopySourceIfNoneMatch, request.IfETagNotMatch);
            request.AddHeader(AmzHeaders.XAmzCopySourceIfModifiedSince, request.IfModifiedSince, DateTimeFormat.Rfc1123);
            request.AddHeader(AmzHeaders.XAmzCopySourceIfUnmodifiedSince, request.IfUnmodifiedSince, DateTimeFormat.Rfc1123);

            if (request.Metadata != null)
            {
                request.AddHeader(AmzHeaders.XAmzMetadataDirective, request.MetadataDirective);

                foreach (KeyValuePair<string, string> item in request.Metadata.GetPrefixed())
                    request.AddHeader(item.Key, item.Value);
            }

            string tags = request.Tags.Build();
            if (tags != null)
            {
                request.AddHeader(AmzHeaders.XAmzTagging, tags);
                request.AddHeader(AmzHeaders.XAmzTaggingDirective, request.TaggingDirective);
            }

            request.AddHeader(AmzHeaders.XAmzCopySourceSseCustomerAlgorithm, request.SseCustomerAlgorithm);
            request.AddHeader(AmzHeaders.XAmzCopySourceSseCustomerKey, request.SseCustomerKey, BinaryEncoding.Base64);
            request.AddHeader(AmzHeaders.XAmzCopySourceSseCustomerKeyMd5, request.SseCustomerKeyMd5, BinaryEncoding.Base64);

            request.AddHeader(AmzHeaders.XAmzSse, request.SseAlgorithm);
            request.AddHeader(AmzHeaders.XAmzSseAwsKmsKeyId, request.SseKmsKeyId);

            string sseContext = request.SseContext.Build();
            if (sseContext != null)
                request.AddHeader(AmzHeaders.XAmzSseContext, Encoding.UTF8.GetBytes(sseContext), BinaryEncoding.Base64);

            request.AddHeader(AmzHeaders.XAmzSseCustomerAlgorithm, request.SseCustomerAlgorithm);
            request.AddHeader(AmzHeaders.XAmzSseCustomerKey, request.SseCustomerKey, BinaryEncoding.Base64);
            request.AddHeader(AmzHeaders.XAmzSseCustomerKeyMd5, request.SseCustomerKeyMd5, BinaryEncoding.Base64);

            request.AddHeader(AmzHeaders.XAmzGrantFullControl, request.AclGrantFullControl);
            request.AddHeader(AmzHeaders.XAmzGrantRead, request.AclGrantRead);
            request.AddHeader(AmzHeaders.XAmzGrantReadAcp, request.AclGrantReadAcp);
            request.AddHeader(AmzHeaders.XAmzGrantWriteAcp, request.AclGrantWriteAcp);

            request.AddHeader(AmzHeaders.XAmzObjectLockMode, request.LockMode);
            request.AddHeader(AmzHeaders.XAmzObjectLockRetainUntilDate, request.LockRetainUntil, DateTimeFormat.Iso8601DateTimeExt);
            request.AddHeader(AmzHeaders.XAmzObjectLockLegalHold, request.LockLegalHold);

            request.AddHeader(AmzHeaders.XAmzWebsiteRedirectLocation, request.WebsiteRedirectLocation);
            request.AddHeader(AmzHeaders.XAmzStorageClass, request.StorageClass);

            return null;
        }
    }
}