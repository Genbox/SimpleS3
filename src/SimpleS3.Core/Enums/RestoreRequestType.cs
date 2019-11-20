using System.Runtime.Serialization;

namespace Genbox.SimpleS3.Core.Enums
{
    public enum RestoreRequestType
    {
        Unknown = 0,

        [EnumMember(Value = "SELECT")]
        Select
    }
}