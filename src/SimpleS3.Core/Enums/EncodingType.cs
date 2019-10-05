using System.Runtime.Serialization;

namespace Genbox.SimpleS3.Core.Enums
{
    /// <summary>Used to specify the encoding of responses when using certain APIs that are based on XML.</summary>
    public enum EncodingType
    {
        Unknown,

        [EnumMember(Value = "url")]
        Url
    }
}