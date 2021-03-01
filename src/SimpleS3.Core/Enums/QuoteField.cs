using Genbox.SimpleS3.Core.Abstracts;

namespace Genbox.SimpleS3.Core.Enums
{
    public enum QuoteField
    {
        Unknown = 0,

        [EnumValue("ALWAYS")]
        Always,

        [EnumValue("ASNEEDED")]
        AsNeeded
    }
}