using System.Runtime.Serialization;

namespace Genbox.SimpleS3.Core.Enums
{
    public enum ExpressionType
    {
        Unknown = 0,

        [EnumMember(Value = "SQL")]
        Sql
    }
}