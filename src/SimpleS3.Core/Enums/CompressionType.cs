using Genbox.SimpleS3.Core.Common;

namespace Genbox.SimpleS3.Core.Enums;

public enum CompressionType
{
    Unknown = 0,

    [EnumValue("NONE")] None,

    [EnumValue("GZIP")] Gzip,

    [EnumValue("BZIP2")] Bzip
}