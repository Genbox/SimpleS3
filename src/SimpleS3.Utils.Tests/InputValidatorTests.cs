using Genbox.SimpleS3.Abstracts.Enums;
using Xunit;

namespace Genbox.SimpleS3.Utils.Tests
{
    public class InputValidatorTests
    {
        [Theory]
        [InlineData("abc")]
        [InlineData("a.b.c.d")]
        [InlineData("1.2.3.4")]
        public void BucketNameSuccessTest(string input)
        {
            Assert.True(InputValidator.TryValidateBucketName(input, out _));
        }

        [Theory]
        [InlineData("a")] //Too short
        [InlineData("a_b")] //Must not contain underscore
        [InlineData("aBc")] //Must not contain uppercase
        [InlineData("-asd")] //Must not start with hyphen
        public void BucketNameFailTest(string input)
        {
            Assert.False(InputValidator.TryValidateBucketName(input, out _));
        }

        [Theory]
        [InlineData("abcdeABCDE01234!-_.*'()", KeyValidationMode.SafeMode)]
        [InlineData("..", KeyValidationMode.SafeMode)]
        [InlineData("4my-organization", KeyValidationMode.SafeMode)]
        [InlineData("my.great_photos-2014/jan/myvacation.jpg", KeyValidationMode.SafeMode)]
        [InlineData("videos/2014/birthday/video1.wmv", KeyValidationMode.SafeMode)]
        [InlineData("this is a test?", KeyValidationMode.AsciiMode)]
        [InlineData("#[]<>", KeyValidationMode.ExtendedAsciiMode)]
        [InlineData("!\"#¤%&/()=?", KeyValidationMode.ExtendedAsciiMode)]
        public void ObjectKeySuccessTest(string input, KeyValidationMode mode)
        {
            Assert.True(InputValidator.TryValidateObjectKey(input, mode, out _));
        }

        [Theory]
        [InlineData(" ", KeyValidationMode.SafeMode)]
        [InlineData(".\\.", KeyValidationMode.SafeMode)]
        [InlineData("a/b[].jpg", KeyValidationMode.SafeMode)]
        public void ObjectKeyFailTest(string input, KeyValidationMode mode)
        {
            Assert.False(InputValidator.TryValidateObjectKey(input, mode, out _));
        }
    }
}