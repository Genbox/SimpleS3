using Genbox.HttpBuilders;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;

namespace Genbox.SimpleS3.Core.Internals.Fluent;

internal class Download : IDownload
{
    private readonly IMultipartTransfer _multipartTransfer;
    private readonly IObjectOperations _operations;
    private readonly GetObjectRequest _request;

    internal Download(IObjectOperations operations, IMultipartTransfer multipartTransfer, string bucket, string objectKey)
    {
        _request = new GetObjectRequest(bucket, objectKey);
        _operations = operations;
        _multipartTransfer = multipartTransfer;
    }

    public IAsyncEnumerable<GetObjectResponse> DownloadMultipartAsync(Stream output, CancellationToken token = default) => _multipartTransfer.MultipartDownloadAsync(_request.BucketName, _request.ObjectKey, output, config: CopyProperties, token: token);

    /// <summary>Enabled Server Side Encryption (SSE) with the provided key.</summary>
    public IDownload WithEncryptionCustomerKey(byte[] encryptionKey)
    {
        _request.SseCustomerAlgorithm = SseCustomerAlgorithm.Aes256;
        _request.SseCustomerKey = encryptionKey;
        _request.SseCustomerKeyMd5 = CryptoHelper.Md5Hash(encryptionKey);
        return this;
    }

    public IDownload WithRange(RangeBuilder builder)
    {
        _request.Range = builder;
        return this;
    }

    public IDownload WithRange(long start, long end)
    {
        _request.Range.Add(start, end);
        return this;
    }

    public IDownload WithMultipart(int partNumber)
    {
        _request.PartNumber = partNumber;
        return this;
    }

    public IDownload WithVersionId(string versionId)
    {
        _request.VersionId = versionId;
        return this;
    }

    public IDownload WithDateTimeConditional(DateTimeOffset? ifModifiedSince = null, DateTimeOffset? ifUnmodifiedSince = null)
    {
        _request.IfModifiedSince = ifModifiedSince;
        _request.IfUnmodifiedSince = ifUnmodifiedSince;
        return this;
    }

    public IDownload WithEtagConditional(ETagBuilder? ifETagMatch = null, ETagBuilder? ifETagNotMatch = null)
    {
        if (ifETagMatch != null)
            _request.IfETagMatch = ifETagMatch;

        if (ifETagNotMatch != null)
            _request.IfETagNotMatch = ifETagNotMatch;

        return this;
    }

    public IDownload WithEtagConditional(string? ifETagMatch = null, string? ifETagNotMatch = null)
    {
        if (ifETagMatch != null)
            _request.IfETagMatch.Set(ifETagMatch);

        if (ifETagNotMatch != null)
            _request.IfETagNotMatch.Set(ifETagNotMatch);

        return this;
    }

    public Task<GetObjectResponse> DownloadAsync(CancellationToken token = default) => _operations.GetObjectAsync(_request, token);

    private void CopyProperties(GetObjectRequest req)
    {
        int? partNum = req.PartNumber;
        req = _request;
        req.PartNumber = partNum;
    }
}