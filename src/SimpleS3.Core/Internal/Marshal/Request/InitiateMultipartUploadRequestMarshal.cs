using System.Collections.Generic;
using System.IO;
using System.Text;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Internal.Constants;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request
{
    [UsedImplicitly]
    internal class InitiateMultipartUploadRequestMarshal : IRequestMarshal<CreateMultipartUploadRequest>
    {
        public Stream MarshalRequest(CreateMultipartUploadRequest request, IS3Config config)
        {
            //This is required for multipart uploads
            request.AddQueryParameter(ObjectParameters.Uploads, string.Empty);

            request.AddHeader(HttpHeaders.CacheControl, request.CacheControl);
            request.AddHeader(AmzHeaders.XAmzStorageClass, request.StorageClass);
            request.AddHeader(AmzHeaders.XAmzTagging, request.Tags);
            request.AddHeader(AmzHeaders.XAmzWebsiteRedirectLocation, request.WebsiteRedirectLocation);
            request.AddHeader(AmzHeaders.XAmzObjectLockMode, request.LockMode);
            request.AddHeader(AmzHeaders.XAmzObjectLockRetainUntilDate, request.LockRetainUntil, DateTimeFormat.Iso8601DateTimeExt);
            request.AddHeader(AmzHeaders.XAmzObjectLockLegalHold, request.LockLegalHold);
            request.AddHeader(AmzHeaders.XAmzAcl, request.Acl);
            request.AddHeader(AmzHeaders.XAmzGrantRead, request.AclGrantRead);
            request.AddHeader(AmzHeaders.XAmzGrantReadAcp, request.AclGrantReadAcp);
            request.AddHeader(AmzHeaders.XAmzGrantWriteAcp, request.AclGrantWriteAcp);
            request.AddHeader(AmzHeaders.XAmzGrantFullControl, request.AclGrantFullControl);
            request.AddHeader(AmzHeaders.XAmzSSE, request.SseAlgorithm);
            request.AddHeader(AmzHeaders.XAmzSSEAwsKmsKeyId, request.SseKmsKeyId);

            string sseContext = request.SseContext.Build();
            if (sseContext != null)
                request.AddHeader(AmzHeaders.XAmzSSEContext, Encoding.UTF8.GetBytes(sseContext), BinaryEncoding.Base64);

            request.AddHeader(AmzHeaders.XAmzSSECustomerAlgorithm, request.SseCustomerAlgorithm);
            request.AddHeader(AmzHeaders.XAmzSSECustomerKey, request.SseCustomerKey, BinaryEncoding.Base64);
            request.AddHeader(AmzHeaders.XAmzSSECustomerKeyMD5, request.SseCustomerKeyMd5, BinaryEncoding.Base64);
            request.AddHeader(HttpHeaders.Expires, request.Expires, DateTimeFormat.Rfc1123);
            request.AddHeader(HttpHeaders.ContentDisposition, request.ContentDisposition);
            request.AddHeader(HttpHeaders.ContentEncoding, request.ContentEncoding);
            request.AddHeader(HttpHeaders.ContentType, request.ContentType);
            request.AddHeader(HttpHeaders.ContentMd5, request.ContentMd5, BinaryEncoding.Base64);

            if (request.Metadata != null)
            {
                foreach (KeyValuePair<string, string> item in request.Metadata.GetPrefixed())
                    request.AddHeader(item.Key, item.Value);
            }

            return null;
        }
    }
}