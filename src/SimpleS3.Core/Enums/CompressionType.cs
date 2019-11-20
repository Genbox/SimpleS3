using System.Runtime.Serialization;

namespace Genbox.SimpleS3.Core.Enums
{
    public enum CompressionType
    {
        Unknown = 0,

        [EnumMember(Value = "NONE")]
        None,

        [EnumMember(Value = "GZIP")]
        Gzip,

        [EnumMember(Value = "BZIP2")]
        Bzip
    }
}