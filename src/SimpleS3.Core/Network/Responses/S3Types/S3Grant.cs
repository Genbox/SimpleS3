using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types
{
    public class S3Grant
    {
        public S3Grant(GrantType type, Permission permission, string? name, string? id, string? uri)
        {
            Name = name;
            Id = id;
            Uri = uri;
            Permission = permission;
            Type = type;
        }

        public string? Name { get; }
        public string? Id { get; }
        public string? Uri { get; }
        public Permission Permission { get; }
        public GrantType Type { get; }
    }
}