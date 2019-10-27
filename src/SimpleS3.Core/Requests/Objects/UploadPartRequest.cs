using System;
using System.IO;
using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Features;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Requests.Objects.Properties;

namespace Genbox.SimpleS3.Core.Requests.Objects
{
    /// <summary>This operation uploads a part in a multipart upload.</summary>
    public class UploadPartRequest : BaseRequest, ISseCustomerKeyProperties, ISupportStreaming
    {
        private byte[] _sseCustomerKey;

        public UploadPartRequest(string bucketName, string objectKey, int partNumber, string uploadId, Stream content) : base(HttpMethod.PUT, bucketName, objectKey)
        {
            if (partNumber <= 0 || partNumber > 10_000)
                throw new ArgumentException("Part number must be between 1 and 10.000 inclusive", nameof(partNumber));

            PartNumber = partNumber;
            UploadId = uploadId;
            Content = content;
        }

        /// <summary>
        /// This header can be used as a message integrity check to verify that the part data is the same data that was originally sent. Although it is
        /// optional, we recommend using the Content-MD5 mechanism as an end-to-end integrity check. For more information, see RFC 1864.
        /// </summary>
        public byte[] ContentMd5 { get; set; }

        /// <summary>The part number</summary>
        public int PartNumber { get; }

        /// <summary>The associated upload id</summary>
        public string UploadId { get; }

        /// <summary>Content of the part</summary>
        public Stream Content { get; }

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
    }
}