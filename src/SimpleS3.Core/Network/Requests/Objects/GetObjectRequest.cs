using System;
using Genbox.HttpBuilders;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Properties;
using Genbox.SimpleS3.Core.Network.SharedProperties;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects
{
    /// <summary>
    /// This implementation of the GET operation retrieves objects from Amazon S3. To use GET, you must have READ access to the object. If you grant
    /// READ access to the anonymous user, you can return the object without using an authorization header.
    /// </summary>
    public class GetObjectRequest : BaseRequest, IHasRange, IHasCache, IHasSseCustomerKey, IHasResponseHeader, IHasVersionId
    {
        private byte[] _sseCustomerKey;

        public GetObjectRequest(string bucketName, string objectKey) : base(HttpMethod.GET, bucketName, objectKey)
        {
            Range = new RangeBuilder();
            IfETagMatch = new ETagBuilder();
            IfETagNotMatch = new ETagBuilder();
            ResponseCacheControl = new CacheControlBuilder();
            ResponseContentType = new ContentTypeBuilder();
            ResponseContentDisposition = new ContentDispositionBuilder();
            ResponseContentLanguage = new ContentLanguageBuilder();
            ResponseContentEncoding = new ContentEncodingBuilder();
        }

        /// <summary>
        /// Part number of the object part being read. This is a positive integer between 1 and the maximum number of parts supported. Only objects
        /// uploaded using the multipart upload API have part numbers. For information about multipart uploads, see Multipart Upload Overview in the Amazon
        /// Simple Storage Service Developer Guide.
        /// </summary>
        public int? PartNumber { get; set; }

        /// <inheritdoc />
        public DateTimeOffset? IfModifiedSince { get; set; }

        /// <inheritdoc />
        public DateTimeOffset? IfUnmodifiedSince { get; set; }

        /// <inheritdoc />
        public ETagBuilder IfETagMatch { get; internal set; }

        /// <inheritdoc />
        public ETagBuilder IfETagNotMatch { get; internal set; }

        /// <inheritdoc />
        public RangeBuilder Range { get; internal set; }

        /// <inheritdoc />
        public DateTimeOffset? ResponseExpires { get; set; }

        /// <inheritdoc />
        public CacheControlBuilder ResponseCacheControl { get; }

        /// <inheritdoc />
        public ContentTypeBuilder ResponseContentType { get; }

        /// <inheritdoc />
        public ContentDispositionBuilder ResponseContentDisposition { get; }

        /// <inheritdoc />
        public ContentLanguageBuilder ResponseContentLanguage { get; }

        /// <inheritdoc />
        public ContentEncodingBuilder ResponseContentEncoding { get; }

        /// <inheritdoc />
        public SseCustomerAlgorithm SseCustomerAlgorithm { get; set; }

        /// <inheritdoc />
        public byte[] SseCustomerKey
        {
            get => _sseCustomerKey;
            set
            {
                if (value == null)
                {
                    _sseCustomerKey = null;
                    return;
                }

                _sseCustomerKey = new byte[value.Length];
                Array.Copy(value, 0, _sseCustomerKey, 0, value.Length);
            }
        }

        /// <inheritdoc />
        public byte[] SseCustomerKeyMd5 { get; set; }

        public void ClearSensitiveMaterial()
        {
            if (_sseCustomerKey != null)
                Array.Clear(_sseCustomerKey, 0, _sseCustomerKey.Length);
        }

        public string VersionId { get; set; }
    }
}