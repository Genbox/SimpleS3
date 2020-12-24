using System;
using System.IO;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Requests.Multipart
{
    /// <summary>This operation uploads a part in a multipart upload.</summary>
    public class UploadPartRequest : BaseRequest, IHasSseCustomerKey, IHasContentMd5, ISupportStreaming, IHasUploadId, IHasRequestPayer, IHasBucketName, IHasObjectKey, IHasPartNumber, IHasContent
    {
        private byte[]? _sseCustomerKey;

        public UploadPartRequest(string bucketName, string objectKey, int partNumber, string uploadId, Stream content) : base(HttpMethod.PUT)
        {
            Initialize(bucketName, objectKey, partNumber, uploadId, content);
        }

        internal void Initialize(string bucketName, string objectKey, int partNumber, string uploadId, Stream content)
        {
            if (partNumber <= 0 || partNumber > 10_000)
                throw new ArgumentException("Part number must be between 1 and 10.000 inclusive", nameof(partNumber));

            BucketName = bucketName;
            ObjectKey = objectKey;
            PartNumber = partNumber;
            UploadId = uploadId;
            Content = content;
        }

        public string BucketName { get; set; }
        public Stream? Content { get; private set; }
        public byte[]? ContentMd5 { get; set; }
        public string ObjectKey { get; set; }

        public int? PartNumber { get; set; }
        public Payer RequestPayer { get; set; }
        public SseCustomerAlgorithm SseCustomerAlgorithm { get; set; }
        public byte[]? SseCustomerKeyMd5 { get; set; }

        public byte[]? SseCustomerKey
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

        public string UploadId { get; set; }

        public override void Reset()
        {
            ContentMd5 = null;
            RequestPayer = Payer.Unknown;
            SseCustomerAlgorithm = SseCustomerAlgorithm.Unknown;
            SseCustomerKey = null;
            SseCustomerKeyMd5 = null;

            base.Reset();
        }
    }
}