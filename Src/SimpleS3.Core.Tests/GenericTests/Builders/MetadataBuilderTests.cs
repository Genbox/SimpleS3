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
}