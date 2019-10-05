using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Requests.Objects.Types
{
    [PublicAPI]
    public class S3DeleteInfo
    {
        public S3DeleteInfo(string name, string versionId)
        {
            Name = name;
            VersionId = versionId;
        }

        public string Name { get; set; }
        public string VersionId { get; set; }
    }
}