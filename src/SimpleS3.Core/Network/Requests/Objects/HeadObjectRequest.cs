using System;
using Genbox.HttpBuilders;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Properties;
using Genbox.SimpleS3.Core.Network.SharedProperties;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects
{
    /// <summary>
    /// The HEAD operation retrieves metadata from an object without returning the object itself. This operation is useful if you are interested
    /// only in an object's metadata. To use HEAD, you must have READ access to the object. A HEAD request has the same options as a GET operation on an
    /// object. The response is identical to the GET response except that there is no response body.
    /// </summary>
    public class HeadObjectRequest : BaseRequest, IHasRange, IHasCache, IHasSseCustomerKey, IHasResponseHeader, IHasVersionId, IHasBucketName, IHasObjectKey
    {
        private byte[] _sseCustomerKey;

        public HeadObjectRequest(string bucketName, string objectKey) : base(HttpMethod.HEAD)
        {
            BucketName = bucketName;
            ObjectKey = objectKey;
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

        public string BucketName { get; set; }

        public DateTimeOffset? IfModifiedSince { get; set; }
        public DateTimeOffset? IfUnmodifiedSince { get; set; }
        public ETagBuilder IfETagMatch { get; internal set; }
        public ETagBuilder IfETagNotMatch { get; internal set; }
        public string ObjectKey { get; set; }
        public RangeBuilder Range { get; internal set; }
        public DateTimeOffset? ResponseExpires { get; set; }
        public CacheControlBuilder ResponseCacheControl { get; }
        public ContentTypeBuilder ResponseContentType { get; }
        public ContentDispositionBuilder ResponseContentDisposition { get; }
        public ContentLanguageBuilder ResponseContentLanguage { get; }
        public ContentEncodingBuilder ResponseContentEncoding { get; }
        public SseCustomerAlgorithm SseCustomerAlgorithm { get; set; }
        public byte[] SseCustomerKeyMd5 { get; set; }

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

        public void ClearSensitiveMaterial()
        {
            if (_sseCustomerKey != null)
                Array.Clear(_sseCustomerKey, 0, _sseCustomerKey.Length);
        }

        public string VersionId { get; set; }
    }
}