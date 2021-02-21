using Genbox.SimpleS3.Core.Abstracts;

namespace Genbox.SimpleS3.Core.Common.Tests
{
    internal enum TestEnum
    {
        Unknown = 0,
        Value1 = 1,
        Value2 = 2,

        [EnumValue("Value3-FromAttribute")]
        Value3 = 3,
    }
}