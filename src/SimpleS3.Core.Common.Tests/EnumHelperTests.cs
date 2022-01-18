using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Tests.Code;
using Xunit;

namespace Genbox.SimpleS3.Core.Common.Tests;

public class EnumHelperTests
{
    [Fact]
    public void AsStringTest()
    {
        Assert.Equal("Value1", EnumHelper.AsString(TestEnum.Value1));
        Assert.Equal("Value3-FromAttribute", EnumHelper.AsString(TestEnum.Value3));
    }

    [Fact]
    public void TryParseTest()
    {
        Assert.True(EnumHelper.TryParse("1", out TestEnum enumVal));
        Assert.Equal(TestEnum.Value1, enumVal);

        Assert.True(EnumHelper.TryParse("Value2", out TestEnum enumVal2));
        Assert.Equal(TestEnum.Value2, enumVal2);

        Assert.True(EnumHelper.TryParse("Value3-FromAttribute", out TestEnum enumVal3));
        Assert.Equal(TestEnum.Value3, enumVal3);

        Assert.False(EnumHelper.TryParse<TestEnum>("NotValid", out _));
    }
}