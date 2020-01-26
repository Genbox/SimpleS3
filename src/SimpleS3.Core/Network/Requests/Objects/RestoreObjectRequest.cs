using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;

namespace Genbox.SimpleS3.Core.Network.Requests.Objects
{
    public class RestoreObjectRequest : BaseRequest, IHasRequestPayer, IHasVersionId, IHasBucketName, IHasObjectKey
    {
        internal RestoreObjectRequest() : base(HttpMethod.POST)
        {
            SelectParameters = new S3SelectParameters();
        }

        public RestoreObjectRequest(string bucketName, string objectKey) : this()
        {
            BucketName = bucketName;
            ObjectKey = objectKey;
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

        public S3SelectParameters SelectParameters { get; private set; }
        public OutputLocation OutputLocation { get; set; }
        public string BucketName { get; set; }
        public string ObjectKey { get; set; }
        public Payer RequestPayer { get; set; }
        public string VersionId { get; set; }

        public override void Reset()
        {
            BucketName = null;
            ObjectKey = null;
            SelectParameters = new S3SelectParameters(); //TODO: Reset better
            Days = 0;
            GlacierTier = RetrievalTier.Unknown;
            RequestTier = RetrievalTier.Unknown;
            RequestType = RestoreRequestType.Unknown;
            Description = null;
            OutputLocation = null;
            RequestPayer = Payer.Unknown;
            VersionId = null;

            base.Reset();
        }
    }
}