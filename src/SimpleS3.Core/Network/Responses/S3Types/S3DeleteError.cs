using Genbox.SimpleS3.Core.Network.SharedProperties;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types
{
    public class S3DeleteError : IHasVersionId
    {
        public string ObjectKey { get; internal set; }
        public string VersionId { get; internal set; }
        public string Code { get; internal set; }
        public string Message { get; internal set; }

        public override string ToString()
        {
            return $"Error on {ObjectKey}: {Message}";
        }
    }
}