using Genbox.SimpleS3.Core.Abstracts;

namespace Genbox.SimpleS3.Core.Enums
{
    public enum ReplicationStatus
    {
        Unknown = 0,

        [EnumValue("PENDING")]
        Pending,

        [EnumValue("COMPLETED")]
        Completed,

        [EnumValue("FAILED")]
        Failed,

        [EnumValue("REPLICA")]
        Replica
    }
}