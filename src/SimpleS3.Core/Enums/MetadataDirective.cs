using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Common;

namespace Genbox.SimpleS3.Core.Enums
{
    public enum MetadataDirective
    {
        Unknown = 0,

        [EnumValue("COPY")]
        Copy,

        [EnumValue("REPLACE")]
        Replace
    }
}