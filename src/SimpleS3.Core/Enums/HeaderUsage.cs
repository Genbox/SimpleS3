using System.Runtime.Serialization;

namespace Genbox.SimpleS3.Core.Enums
{
    public enum HeaderUsage
    {
        Unknown = 0,

        [EnumMember(Value = "NONE")]
        None,

        [EnumMember(Value = "IGNORE")]
        Ignore,

        [EnumMember(Value = "USE")]
        Use
    }
}