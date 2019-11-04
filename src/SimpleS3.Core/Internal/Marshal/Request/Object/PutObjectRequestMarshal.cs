using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Constants;
using Genbox.SimpleS3.Abstracts.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Enums;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Internal.Marshal.Request.Object
{
    [UsedImplicitly]
    internal class PutObjectRequestMarshal : IRequestMarshal<PutObjectRequest>
    {
        public Stream MarshalRequest(PutObjectRequest request, IS3Config config)
        {
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
            request.AddHeader(HttpHeaders.Expires, request.ExpiresOn, DateTimeFormat.Rfc1123);
            request.AddHeader(HttpHeaders.ContentDisposition, request.ContentDisposition);
            request.AddHeader(HttpHeaders.ContentEncoding, request.ContentEncoding);
            request.AddHeader(HttpHeaders.ContentType, request.ContentType);
            request.AddHeader(AmzHeaders.XAmzRequestPayer, request.RequestPayer == Payer.Requester ? "requester" : null);

            if (request.Metadata != null)
            {
                foreach (KeyValuePair<string, string> item in request.Metadata.GetPrefixed())
                    request.AddHeader(item.Key, item.Value);
            }

            //Locks require the Content-MD5 header to be set
            if (request.LockMode != LockMode.Unknown || (request.LockLegalHold.HasValue && request.LockLegalHold.Value))
            {
                string md5Hash = request.Content == null ? "1B2M2Y8AsgTpgAmY7PhCfg==" : Convert.ToBase64String(CryptoHelper.Md5Hash(request.Content, true));
                request.AddHeader(HttpHeaders.ContentMd5, md5Hash);
            }
            else
                request.AddHeader(HttpHeaders.ContentMd5, request.ContentMd5, BinaryEncoding.Base64);

            return request.Content;
        }
    }
}