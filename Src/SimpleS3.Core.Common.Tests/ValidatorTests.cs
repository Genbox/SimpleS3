using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Core.Common.Tests;

public class ValidatorTests
{
    [Fact]
    public void RequireThatContainsUsefulDataTest()
    {
        try
        {
            Validator.RequireThat(1 == 2, "this is a message");
        }
        catch (RequireException e)
        {
            Assert.Equal("1 == 2", e.CallerArgument);
            Assert.Equal(nameof(RequireThatContainsUsefulDataTest), e.CallerMember);
            Assert.Equal(12, e.LineNumber);
            Assert.Equal("this is a message", e.Message);
        }
    }

    [Fact]
    public void RequireNotNullContainsUsefulDataTest()
    {
        try
        {
            Validator.RequireNotNull((string?)null, "this is a message");
        }
        catch (RequireException e)
        {
            Assert.Equal("(string?)null", e.CallerArgument);
            Assert.Equal(nameof(RequireNotNullContainsUsefulDataTest), e.CallerMember);
            Assert.Equal(28, e.LineNumber);
            Assert.Equal("this is a message", e.Message);
        }
    }

    [Fact]
    public void RequireNotNullOrEmptyTest()
    {
        Assert.Throws<RequireException>(() => Validator.RequireNotNullOrEmpty((string?)null));
        Assert.Throws<RequireException>(() => Validator.RequireNotNullOrEmpty(string.Empty));
    }

    [Fact]
    public void RequireNotNullTest()
    {
        object? obj = null;
        Assert.Throws<RequireException>(() => Validator.RequireNotNull(obj));
    }
}