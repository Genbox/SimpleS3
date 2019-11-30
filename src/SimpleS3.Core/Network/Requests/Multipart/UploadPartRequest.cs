using System;
using System.IO;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Properties;
using Genbox.SimpleS3.Core.Network.SharedProperties;

namespace Genbox.SimpleS3.Core.Network.Requests.Multipart
{
    /// <summary>This operation uploads a part in a multipart upload.</summary>
    public class UploadPartRequest : BaseRequest, IHasSseCustomerKey, IHasContentMd5, ISupportStreaming, IHasUploadId, IHasRequestPayer, IHasBucketName, IHasObjectKey
    {
        private byte[] _sseCustomerKey;

        public UploadPartRequest(string bucketName, string objectKey, int partNumber, string uploadId, Stream content) : base(HttpMethod.PUT)
        {
            if (partNumber <= 0 || partNumber > 10_000)
                throw new ArgumentException("Part number must be between 1 and 10.000 inclusive", nameof(partNumber));

            BucketName = bucketName;
            ObjectKey = objectKey;
            PartNumber = partNumber;
            UploadId = uploadId;
            Content = content;
        }

        /// <summary>The part number</summary>
        public int PartNumber { get; }

        /// <summary>Content of the part</summary>
        public Stream Content { get; }

        public string BucketName { get; set; }

        public byte[] ContentMd5 { get; set; }
        public string ObjectKey { get; set; }
        public Payer RequestPayer { get; set; }
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

        public string UploadId { get; }
    }
}