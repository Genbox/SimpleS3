using System.Runtime.Serialization;

namespace Genbox.SimpleS3.Core.Enums
{
    public enum S3Permission
    {
        Unknown = 0,

        [EnumMember(Value = "FULL_CONTROL")]
        FullControl,

        [EnumMember(Value = "READ")]
        Read,

        [EnumMember(Value = "WRITE")]
        Write
    }
}