using Genbox.SimpleS3.Core.Common;

namespace Genbox.SimpleS3.Core.Enums;

public enum JsonType
{
    Unknown = 0,

    [EnumValue("DOCUMENT")] Document,

    [EnumValue("LINES")] Lines
}