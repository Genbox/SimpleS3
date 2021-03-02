using System;
using System.Threading;
using System.Threading.Tasks;
using Genbox.HttpBuilders;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
#if COMMERCIAL
using System.Collections.Generic;
using System.IO;
#endif

namespace Genbox.SimpleS3.Core.Abstracts
{
    public interface IDownload
    {
#if COMMERCIAL
        IAsyncEnumerable<GetObjectResponse> DownloadMultipartAsync(Stream output, CancellationToken token = default);
#endif

        /// <summary>Enabled Server Side Encryption (SSE) with the provided key.</summary>
        IDownload WithEncryptionCustomerKey(byte[] encryptionKey);

        IDownload WithRange(RangeBuilder builder);
        IDownload WithRange(long start, long end);
        IDownload WithMultipart(int partNumber);
        IDownload WithVersionId(string versionId);
        IDownload WithDateTimeConditional(DateTimeOffset? ifModifiedSince = null, DateTimeOffset? ifUnmodifiedSince = null);
        IDownload WithEtagConditional(ETagBuilder? ifETagMatch = null, ETagBuilder? ifETagNotMatch = null);
        IDownload WithEtagConditional(string? ifETagMatch = null, string? ifETagNotMatch = null);
        Task<GetObjectResponse> DownloadAsync(CancellationToken token = default);
    }
}