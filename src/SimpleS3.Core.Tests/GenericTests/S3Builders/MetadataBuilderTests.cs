using System;
using System.Globalization;
using Genbox.SimpleS3.Core.Builders;
using Xunit;

namespace Genbox.SimpleS3.Core.Tests.GenericTests.S3Builders
{
    public class MetadataBuilderTests
    {
        [Fact]
        public void InvalidChars()
        {
            MetadataBuilder b = new MetadataBuilder();

            string invalidChars = "\t\0\n";

            foreach (char invalidChar in invalidChars)
                Assert.Throws<ArgumentException>(() => { b.Add("a", invalidChar.ToString(CultureInfo.InvariantCulture)); });
        }
    }
}