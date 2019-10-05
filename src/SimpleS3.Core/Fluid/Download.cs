using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genbox.HttpBuilders;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Genbox.SimpleS3.Core.Misc;
using Genbox.SimpleS3.Core.Requests.Objects;
using Genbox.SimpleS3.Core.Responses.Objects;

namespace Genbox.SimpleS3.Core.Fluid
{
    public class Download
    {
        private readonly IObjectOperations _objectOperations;
        private readonly GetObjectRequest _request;

        internal Download(IObjectOperations objectOperations, string bucket, string resource)
        {
            _request = new GetObjectRequest(bucket, resource);
            _objectOperations = objectOperations;
        }

        public async Task<MultipartDownloadStatus> ExecuteMultipartAsync(Stream output, CancellationToken token = default)
        {
            IAsyncEnumerable<GetObjectResponse> async = _objectOperations.MultipartDownloadAsync(_request.BucketName, _request.Resource, output, config: CopyProperties, token: token);

            await foreach (GetObjectResponse resp in async.WithCancellation(token))
            {
                if (!resp.IsSuccess)
                    return MultipartDownloadStatus.Incomplete;
            }

            return MultipartDownloadStatus.Ok;
        }

        /// <summary>Enabled Server Side Encryption (SSE) with the provided key.</summary>
        public Download WithEncryptionCustomerKey(byte[] encryptionKey)
        {
            _request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
            _request.SseCustomerKey = encryptionKey;
            _request.SseCustomerKeyMd5 = CryptoHelper.Md5Hash(encryptionKey);
            return this;
        }

        public Download WithRange(RangeBuilder builder)
        {
            _request.Range = builder;
            return this;
        }

        public Download WithRange(long start, long end)
        {
            _request.Range.Add(start, end);
            return this;
        }

        public Download WithMultipart(int partNumber)
        {
            _request.PartNumber = partNumber;
            return this;
        }

        public Download WithVersionId(string versionId)
        {
            _request.VersionId = versionId;
            return this;
        }

        public Download WithDateTimeConditional(DateTimeOffset? ifModifiedSince = null, DateTimeOffset? ifUnmodifiedSince = null)
        {
            _request.IfModifiedSince = ifModifiedSince;
            _request.IfUnmodifiedSince = ifUnmodifiedSince;
            return this;
        }

        public Download WithEtagConditional(ETagBuilder ifETagMatch = null, ETagBuilder ifETagNotMatch = null)
        {
            _request.IfETagMatch = ifETagMatch;
            _request.IfETagNotMatch = ifETagNotMatch;
            return this;
        }

        public Download WithEtagConditional(string ifETagMatch = null, string ifETagNotMatch = null)
        {
            _request.IfETagMatch.Set(ifETagMatch);
            _request.IfETagNotMatch.Set(ifETagNotMatch);
            return this;
        }

        private void CopyProperties(GetObjectRequest req)
        {
            int? partNum = req.PartNumber;
            req = _request;
            req.PartNumber = partNum;
        }

        public Task<GetObjectResponse> ExecuteAsync(CancellationToken token = default)
        {
            return _objectOperations.GetAsync(_request, token);
        }
    }
}