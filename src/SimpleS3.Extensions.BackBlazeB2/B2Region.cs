using System.Runtime.Serialization;

namespace Genbox.SimpleS3.Extensions.BackBlazeB2
{
    public enum B2Region
    {
        Unknown = 0,

        [EnumMember(Value = "us-west-001")]
        UsWest001,

        [EnumMember(Value = "us-west-002")]
        UsWest002
    }
}