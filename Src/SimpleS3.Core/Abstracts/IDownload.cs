using Genbox.HttpBuilders;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Abstracts;

/// <summary>Provides the <c>IDownload</c> API.</summary>
public interface IDownload
{
    /// <summary>Provides the <c>DownloadMultipartAsync</c> API.</summary>
    /// <remarks>The helper selects all parts. Combining this operation with WithRange or WithMultipart throws before transport; use DownloadAsync for an explicitly selected range or part.</remarks>
    IAsyncEnumerable<GetObjectResponse> DownloadMultipartAsync(Stream output, CancellationToken token = default);

    /// <summary>Enabled Server Side Encryption (SSE) with the provided key.</summary>
    IDownload WithEncryptionCustomerKey(byte[] encryptionKey);

    /// <summary>Provides the <c>WithRange</c> API.</summary>
    IDownload WithRange(RangeBuilder builder);

    /// <summary>Provides the <c>WithRange</c> API.</summary>
    IDownload WithRange(long start, long end);

    /// <summary>Provides the <c>WithMultipart</c> API.</summary>
    IDownload WithMultipart(int partNumber);

    /// <summary>Provides the <c>WithVersionId</c> API.</summary>
    IDownload WithVersionId(string versionId);

    /// <summary>Provides the <c>WithDateTimeConditional</c> API.</summary>
    IDownload WithDateTimeConditional(DateTimeOffset? ifModifiedSince = null, DateTimeOffset? ifUnmodifiedSince = null);

    /// <summary>Provides the <c>WithEtagConditional</c> API.</summary>
    IDownload WithEtagConditional(ETagBuilder? ifETagMatch = null, ETagBuilder? ifETagNotMatch = null);

    /// <summary>Provides the <c>WithEtagConditional</c> API.</summary>
    IDownload WithEtagConditional(string? ifETagMatch = null, string? ifETagNotMatch = null);

    /// <summary>Provides the <c>DownloadAsync</c> API.</summary>
    Task<GetObjectResponse> DownloadAsync(CancellationToken token = default);
}