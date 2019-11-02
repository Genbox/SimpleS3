using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Properties;
using Genbox.SimpleS3.Core.Network.SharedProperties;

namespace Genbox.SimpleS3.Core.Network.Requests.Multipart
{
    /// <summary>This operation lists the parts that have been uploaded for a specific multipart upload.</summary>
    public class ListPartsRequest : BaseRequest, IHasUploadId, IHasRequestPayer
    {
        public ListPartsRequest(string bucketName, string objectKey, string uploadId) : base(HttpMethod.GET, bucketName, objectKey)
        {
            UploadId = uploadId;
        }

        /// <summary>Requests Amazon S3 to encode the response and specifies the encoding method to use.</summary>
        public EncodingType EncodingType { get; set; }

        /// <summary>Sets the maximum number of parts to return in the response body.</summary>
        public int? MaxParts { get; set; }

        /// <summary>Specifies the part after which listing should begin. Only parts with higher part numbers will be listed.</summary>
        public string PartNumberMarker { get; set; }

        public string UploadId { get; }
        public Payer RequestPayer { get; set; }
    }
}