using Genbox.SimpleS3.Core.Common;

namespace Genbox.SimpleS3.Core.Enums;

public enum TaggingDirective
{
    Unknown = 0,

    [EnumValue("COPY")]Copy,

    [EnumValue("REPLACE")]Replace
}