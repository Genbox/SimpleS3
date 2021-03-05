using Genbox.SimpleS3.Core.Common.Marshal;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types
{
    [PublicAPI]
    public class S3DeleteInfo : IHasObjectKey, IHasVersionId
    {
        public S3DeleteInfo(string objectKey, string? versionId = null)
        {
            ObjectKey = objectKey;
            VersionId = versionId;
        }

        public string ObjectKey { get; set; }
        public string? VersionId { get; set; }
    }
}