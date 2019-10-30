using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Responses.Properties;
using Genbox.SimpleS3.Core.Network.SharedProperties;

namespace Genbox.SimpleS3.Core.Network.Responses.Objects
{
    public class CompleteMultipartUploadResponse : BaseResponse, IHasRequestCharged, IHasVersionId, IHasExpiresOn, IHasETag, IHasSse
    {
        /// <summary>
        /// The URI that identifies the newly created object.
        /// </summary>
        public string Location { get; internal set; }

        /// <summary>
        /// The name of the bucket that contains the newly created object.
        /// </summary>
        public string BucketName { get; internal set; }

        /// <summary>
        /// The object key of the newly created object.
        /// </summary>
        public string ObjectKey { get; internal set; }

        public string ExpiresOn { get; internal set; }

        public bool RequestCharged { get; internal set; }

        public string ETag { get; internal set; }

        public string VersionId { get; internal set; }

        public SseAlgorithm SseAlgorithm { get; internal set; }

        public string SseKmsKeyId { get; internal set; }
    }
}