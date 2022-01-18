using Genbox.SimpleS3.Core.Common.Tests.Code;
using Genbox.SimpleS3.Core.Common.Validation;
using Xunit;

namespace Genbox.SimpleS3.Core.Common.Tests;

public class ValidatorTests
{
    [Fact]
    public void RequireNotNullOrEmptyTest()
    {
        Assert.Throws<ArgumentNullException>(() => Validator.RequireNotNullOrEmpty((string)null!, "prop"));
        Assert.Throws<ArgumentNullException>(() => Validator.RequireNotNullOrEmpty(string.Empty, "prop"));
    }

    [Fact]
    public void RequireNotNullTest()
    {
        object? obj = null;
        Assert.Throws<ArgumentNullException>(() => Validator.RequireNotNull(obj, "prop"));
    }

    [Fact]
    public void RequireValidEnumTest()
    {
        Assert.Throws<ArgumentException>(() => Validator.RequireValidEnum((TestEnum)4, "prop"));
    }
}