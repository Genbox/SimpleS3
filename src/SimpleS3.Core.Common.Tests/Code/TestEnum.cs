namespace Genbox.SimpleS3.Core.Common.Tests.Code;

internal enum TestEnum
{
    Unknown = 0,
    Value1 = 1,
    Value2 = 2,

    [EnumValue("Value3-FromAttribute")]
    Value3 = 3,
}