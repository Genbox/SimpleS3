using System.Runtime.Serialization;

namespace Genbox.SimpleS3.Core.Enums
{
    public enum TaggingDirective
    {
        Unknown = 0,

        [EnumMember(Value = "COPY")]
        Copy,

        [EnumMember(Value = "REPLACE")]
        Replace
    }
}