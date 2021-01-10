using Genbox.SimpleS3.Core.Network.Responses.Interfaces;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types
{
    public class S3DeletedObject : IHasDeleteMarker, IHasVersionId
    {
        public S3DeletedObject(string objectKey, string deleteMarkerVersionId, bool isDeleteMarker, string? versionId)
        {
            ObjectKey = objectKey;
            DeleteMarkerVersionId = deleteMarkerVersionId;
            IsDeleteMarker = isDeleteMarker;
            VersionId = versionId;
        }

        /// <summary>The name of the deleted object.</summary>
        public string ObjectKey { get; }

        /// <summary>
        /// The version ID of the delete marker created as a result of the DELETE operation. If you delete a specific object version, the value returned
        /// by this header is the version ID of the object version deleted.
        /// </summary>
        public string DeleteMarkerVersionId { get; }

        public bool IsDeleteMarker { get; }

        /// <summary>The version ID of the deleted object.</summary>
        public string? VersionId { get; }

        public override string ToString()
        {
            return ObjectKey;
        }
    }
}