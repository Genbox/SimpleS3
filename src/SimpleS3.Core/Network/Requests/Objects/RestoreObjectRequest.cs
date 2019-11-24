using Genbox.SimpleS3.Abstracts.Enums;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Properties;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.SharedProperties;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects
{
    public class RestoreObjectRequest : BaseRequest, IHasRequestPayer, IHasVersionId, IHasBucketName, IHasObjectKey
    {
        public RestoreObjectRequest(string bucketName, string objectKey) : base(HttpMethod.POST)
        {
            BucketName = bucketName;
            ObjectKey = objectKey;
            SelectParameters = new S3SelectParameters();
        }

        /// <summary>
        /// The number of days that you want the restored copy to exist. After the specified period, Amazon S3 deletes the temporary copy but the object
        /// remains archived in the <see cref="StorageClass.Glacier" /> or <see cref="StorageClass.DeepArchive" /> storage class that object was restored from.
        /// Do not use with restores that specify OutputLocation.
        /// </summary>
        public int Days { get; set; }

        /// <summary>Set the tier you want the Glacier job to be handled with. Do not use with restores that specify OutputLocation.</summary>
        public RetrievalTier GlacierTier { get; set; }

        /// <summary>Glacier retrieval tier at which the restore will be processed.</summary>
        public RetrievalTier RequestTier { get; set; }

        public RestoreRequestType RequestType { get; set; }

        /// <summary>The optional description for the job.</summary>
        public string Description { get; set; }

        public S3SelectParameters SelectParameters { get; }
        public OutputLocation OutputLocation { get; set; }

        public Payer RequestPayer { get; set; }
        public string VersionId { get; }
        public string ObjectKey { get; set; }
        public string BucketName { get; set; }
    }
}