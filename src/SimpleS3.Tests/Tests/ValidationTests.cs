using Genbox.SimpleS3.Core.Requests.Objects;
using Genbox.SimpleS3.Core.Validation.Validators.Requests;
using Xunit;

namespace Genbox.SimpleS3.Tests.Tests
{
    public class ValidationTests
    {
        [Theory]
        [InlineData("a")]
        [InlineData("a.b.c.d")]
        [InlineData("1.2.3.4")]
        public void DnsLabelSuccessTest(string input)
        {
            Assert.True(BaseRequestValidator<GetObjectRequest>.BeValidDns(input));
        }

        [Theory]
        [InlineData("a_b")] //Must not contain underscore
        [InlineData("aBc")] //Must not contain uppercase
        [InlineData("-asd")] //Must not start with hyphen
        public void DnsLabelFailTest(string input)
        {
            Assert.False(BaseRequestValidator<GetObjectRequest>.BeValidDns(input));
        }
    }
}