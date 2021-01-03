using System;
using Genbox.HttpBuilders;
using Genbox.HttpBuilders.BuilderOptions;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects
{
    /// <summary>
    /// The HEAD operation retrieves metadata from an object without returning the object itself. This operation is useful if you are interested
    /// only in an object's metadata. To use HEAD, you must have READ access to the object. A HEAD request has the same options as a GET operation on an
    /// object. The response is identical to the GET response except that there is no response body.
    /// </summary>
    public class HeadObjectRequest : BaseRequest, IHasRange, IHasCache, IHasSseCustomerKey, IHasResponseHeader, IHasVersionId, IHasBucketName, IHasObjectKey, IHasPartNumber
    {
        private byte[]? _sseCustomerKey;

        internal HeadObjectRequest() : this(HttpMethod.HEAD) { }

        internal HeadObjectRequest(HttpMethod method) : base(method)
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

        public HeadObjectRequest(string bucketName, string objectKey) : this()
        {
            Initialize(bucketName, objectKey);
        }

        public string BucketName { get; set; }
        public DateTimeOffset? IfModifiedSince { get; set; }
        public DateTimeOffset? IfUnmodifiedSince { get; set; }
        public ETagBuilder IfETagMatch { get; internal set; }
        public ETagBuilder IfETagNotMatch { get; internal set; }
        public string ObjectKey { get; set; }
        public int? PartNumber { get; set; }
        public RangeBuilder Range { get; internal set; }
        public DateTimeOffset? ResponseExpires { get; set; }
        public CacheControlBuilder ResponseCacheControl { get; }
        public ContentTypeBuilder ResponseContentType { get; }
        public ContentDispositionBuilder ResponseContentDisposition { get; }
        public ContentLanguageBuilder ResponseContentLanguage { get; }
        public ContentEncodingBuilder ResponseContentEncoding { get; }
        public SseCustomerAlgorithm SseCustomerAlgorithm { get; set; }
        public byte[]? SseCustomerKeyMd5 { get; set; }

        public byte[]? SseCustomerKey
        {
            get => _sseCustomerKey;
            set => _sseCustomerKey = CopyHelper.CopyWithNull(value);
        }

        public void ClearSensitiveMaterial()
        {
            if (_sseCustomerKey != null)
            {
                Array.Clear(_sseCustomerKey, 0, _sseCustomerKey.Length);
                _sseCustomerKey = null;
            }
        }

        public string? VersionId { get; set; }

        internal void Initialize(string bucketName, string objectKey)
        {
            BucketName = bucketName;
            ObjectKey = objectKey;
        }

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

            base.Reset();
        }
    }
}