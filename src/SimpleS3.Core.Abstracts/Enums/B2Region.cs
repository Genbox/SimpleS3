using System.Runtime.Serialization;

namespace Genbox.SimpleS3.Core
{
    public enum B2Region
    {
        Unknown,

        [EnumMember(Value = "us-west-001")]
        UsWest001,

        [EnumMember(Value = "us-west-002")]
        UsWest002
    }
}