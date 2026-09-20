using Genbox.HttpBuilders;
using Genbox.HttpBuilders.BuilderOptions;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects;

/// <summary>
/// The HEAD operation retrieves metadata from an object without returning the object itself. This operation is useful if you are interested only in an object's metadata. To
/// use HEAD, you must have READ access to the object. A HEAD request has the same options as a GET operation on an object. The response is identical to the GET response except that
/// there is no response body.
/// </summary>
public class HeadObjectRequest : BaseRequest, IHasRange, IHasCache, IHasSseCustomerKey, IHasResponseHeader, IHasVersionId, IHasBucketName, IHasObjectKey, IHasPartNumber
{
    private byte[]? _sseCustomerKey;

    internal HeadObjectRequest() : this(HttpMethodType.HEAD) {}

    internal HeadObjectRequest(HttpMethodType method) : base(method)
    {
        Range = new RangeBuilder();
        IfETagMatch = new ETagBuilder();
        IfETagNotMatch = new ETagBuilder();
        ResponseCacheControl = new CacheControlBuilder();
        ResponseContentType = new ContentTypeBuilder();

        //Amazon does not support the extended filename RFC in their presigned requests
        ContentDispositionOptions contentDisp = new ContentDispositionOptions();
        contentDisp.UseExtendedFilename = false;

        ResponseContentDisposition = new ContentDispositionBuilder(Options.Create(contentDisp));
        ResponseContentLanguage = new ContentLanguageBuilder();
        ResponseContentEncoding = new ContentEncodingBuilder();
    }

    /// <summary>Provides the <c>HeadObjectRequest</c> API.</summary>
    public HeadObjectRequest(string bucketName, string objectKey) : this()
    {
        Initialize(bucketName, objectKey);
    }

    /// <summary>Provides the <c>EnableChecksum</c> API.</summary>
    public bool EnableChecksum { get; set; }

    /// <summary>Provides the <c>BucketName</c> API.</summary>
    public string BucketName { get; set; } = null!;

    /// <summary>Provides the <c>IfModifiedSince</c> API.</summary>
    public DateTimeOffset? IfModifiedSince { get; set; }

    /// <summary>Provides the <c>IfUnmodifiedSince</c> API.</summary>
    public DateTimeOffset? IfUnmodifiedSince { get; set; }

    /// <summary>Provides the <c>IfETagMatch</c> API.</summary>
    public ETagBuilder IfETagMatch { get; internal set; }

    /// <summary>Provides the <c>IfETagNotMatch</c> API.</summary>
    public ETagBuilder IfETagNotMatch { get; internal set; }

    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    public string ObjectKey { get; set; } = null!;

    /// <summary>Provides the <c>PartNumber</c> API.</summary>
    public int? PartNumber { get; set; }

    /// <summary>Provides the <c>Range</c> API.</summary>
    public RangeBuilder Range { get; internal set; }

    /// <summary>Provides the <c>ResponseExpires</c> API.</summary>
    public DateTimeOffset? ResponseExpires { get; set; }

    /// <summary>Provides the <c>ResponseCacheControl</c> API.</summary>
    public CacheControlBuilder ResponseCacheControl { get; }

    /// <summary>Provides the <c>ResponseContentType</c> API.</summary>
    public ContentTypeBuilder ResponseContentType { get; }

    /// <summary>Provides the <c>ResponseContentDisposition</c> API.</summary>
    public ContentDispositionBuilder ResponseContentDisposition { get; }

    /// <summary>Provides the <c>ResponseContentLanguage</c> API.</summary>
    public ContentLanguageBuilder ResponseContentLanguage { get; }

    /// <summary>Provides the <c>ResponseContentEncoding</c> API.</summary>
    public ContentEncodingBuilder ResponseContentEncoding { get; }

    /// <summary>Provides the <c>SseCustomerAlgorithm</c> API.</summary>
    public SseCustomerAlgorithm SseCustomerAlgorithm { get; set; }

    /// <summary>Provides the <c>SseCustomerKeyMd5</c> API.</summary>
    public byte[]? SseCustomerKeyMd5 { get; set; }

    /// <summary>Provides the <c>SseCustomerKey</c> API.</summary>
    public byte[]? SseCustomerKey
    {
        get => _sseCustomerKey;
        set => _sseCustomerKey = CopyHelper.CopyWithNull(value);
    }

    /// <summary>Provides the <c>ClearSensitiveMaterial</c> API.</summary>
    public void ClearSensitiveMaterial()
    {
        if (_sseCustomerKey != null)
        {
            Array.Clear(_sseCustomerKey, 0, _sseCustomerKey.Length);
            _sseCustomerKey = null;
        }
    }

    /// <summary>Provides the <c>VersionId</c> API.</summary>
    public string? VersionId { get; set; }

    internal void Initialize(string bucketName, string objectKey)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
    }

    /// <summary>Provides the <c>Reset</c> API.</summary>
    public override void Reset()
    {
        ClearSensitiveMaterial();

        Range.Reset();
        IfETagMatch.Reset();
        IfETagNotMatch.Reset();
        ResponseCacheControl.Reset();
        ResponseContentType.Reset();
        ResponseContentDisposition.Reset();
        ResponseContentLanguage.Reset();
        ResponseContentEncoding.Reset();
        VersionId = null;
        IfModifiedSince = null;
        IfUnmodifiedSince = null;
        PartNumber = null;
        SseCustomerAlgorithm = SseCustomerAlgorithm.Unknown;
        SseCustomerKeyMd5 = null;
        ResponseExpires = null;
        EnableChecksum = false;

        base.Reset();
    }
}