using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common;

namespace Genbox.SimpleS3.Core.Enums
{
    public enum HeaderUsage
    {
        Unknown = 0,

        [EnumValue("NONE")]
        None,

        [EnumValue("IGNORE")]
        Ignore,

        [EnumValue("USE")]
        Use
    }
}