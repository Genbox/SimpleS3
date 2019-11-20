using System.Runtime.Serialization;

namespace Genbox.SimpleS3.Core.Enums
{
    public enum JsonType
    {
        Unknown = 0,

        [EnumMember(Value = "DOCUMENT")]
        Document,

        [EnumMember(Value = "LINES")]
        Lines
    }
}