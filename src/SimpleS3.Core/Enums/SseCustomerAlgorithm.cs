using System.Runtime.Serialization;

namespace Genbox.SimpleS3.Core.Enums
{
    public enum SseCustomerAlgorithm
    {
        Unknown = 0,

        [EnumMember(Value = "AES256")]
        Aes256
    }
}