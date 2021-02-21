using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common;

namespace Genbox.SimpleS3.Core.Enums
{
    public enum S3Permission
    {
        Unknown = 0,

        [EnumValue("FULL_CONTROL")]
        FullControl,

        [EnumValue("READ")]
        Read,

        [EnumValue("WRITE")]
        Write
    }
}