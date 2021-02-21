using System.Runtime.Serialization;

namespace Genbox.SimpleS3.Core.Benchmarks
{
    internal enum TestEnum
    {
        Unknown = 0,
        Value1 = 1,
        Value2 = 2,

        [EnumMember(Value = "Value3")]
        Value3 = 3,
    }
}