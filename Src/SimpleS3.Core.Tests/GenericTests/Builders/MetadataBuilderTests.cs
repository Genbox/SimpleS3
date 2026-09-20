using System.Globalization;
using Genbox.SimpleS3.Core.Builders;

namespace Genbox.SimpleS3.Core.Tests.GenericTests.Builders;

public class MetadataBuilderTests
{
    [Fact]
    public void InvalidChars()
    {
        MetadataBuilder b = new MetadataBuilder();

        foreach (char invalidChar in "\t\0\n")
            Assert.Throws<ArgumentException>(() => { b.Add("a", invalidChar.ToString(CultureInfo.InvariantCulture)); });
    }

    [Theory]
    [InlineData("space key")]
    [InlineData("colon:key")]
    [InlineData("")]
    public void InvalidHeaderNames(string key)
    {
        MetadataBuilder builder = new MetadataBuilder();
        Assert.Throws<ArgumentException>(() => builder.Add(key, "value"));
    }

    [Fact]
    public void FailedDuplicateDoesNotIncreaseTrackedSize()
    {
        MetadataBuilder builder = new MetadataBuilder();
        builder.Add("a", new string('x', 2046));

        Assert.Throws<ArgumentException>(() => builder.Add("a", string.Empty));
        builder.Add("b", string.Empty);
    }
}