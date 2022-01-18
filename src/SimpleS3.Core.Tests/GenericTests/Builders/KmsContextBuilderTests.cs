using Genbox.SimpleS3.Core.Builders;
using Xunit;

namespace Genbox.SimpleS3.Core.Tests.GenericTests.Builders;

public class KmsContextBuilderTests
{
    [Fact]
    public void DisallowSpecialChars()
    {
        KmsContextBuilder b = new KmsContextBuilder();
        Assert.Throws<ArgumentException>(() => b.AddEntry("!", "¤"));
    }

    [Fact]
    public void GenericTest()
    {
        KmsContextBuilder b = new KmsContextBuilder();
        Assert.Null(b.Build());

        b.AddEntry("SomeKey", "SomeValue");

        Assert.Equal("\"SomeKey\":\"SomeValue\"", b.Build());

        b.AddEntry("SomeOtherKey", "SomeOtherValue");

        Assert.Equal("\"SomeKey\":\"SomeValue\",\"SomeOtherKey\":\"SomeOtherValue\"", b.Build());
    }
}