using Genbox.SimpleS3.Core.Abstracts;

namespace Genbox.SimpleS3.Core.Enums
{
    public enum RestoreRequestType
    {
        Unknown = 0,

        [EnumValue("SELECT")]
        Select
    }
}