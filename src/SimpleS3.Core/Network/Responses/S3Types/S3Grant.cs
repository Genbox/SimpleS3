using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types
{
    public class S3Grant
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
        public string Uri { get; set; }
        public Permission Permission { get; set; }
        public GrantType Type { get; set; }
    }
}