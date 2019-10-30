using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types
{
    [PublicAPI]
    public class S3DeleteInfo
    {
        public S3DeleteInfo(string name, string versionId = null)
        {
            Name = name;
            VersionId = versionId;
        }

        public string Name { get; set; }
        public string VersionId { get; set; }
    }
}