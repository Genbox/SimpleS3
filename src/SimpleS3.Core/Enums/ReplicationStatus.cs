using System.Runtime.Serialization;

namespace Genbox.SimpleS3.Core.Enums
{
    public enum ReplicationStatus
    {
        Unknown = 0,

        [EnumMember(Value = "PENDING")]
        Pending,

        [EnumMember(Value = "COMPLETED")]
        Completed,

        [EnumMember(Value = "FAILED")]
        Failed,

        [EnumMember(Value = "REPLICA")]
        Replica
    }
}