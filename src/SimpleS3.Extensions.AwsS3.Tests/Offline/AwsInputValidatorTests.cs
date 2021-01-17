using Genbox.SimpleS3.Core.Abstracts.Enums;
using Xunit;

namespace Genbox.SimpleS3.Extensions.AwsS3.Tests.Offline
{
    public class AwsInputValidatorTests
    {
        private readonly AwsInputValidator _validator;

        public AwsInputValidatorTests()
        {
            _validator = new AwsInputValidator();
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("a.b.c.d")]
        [InlineData("1.2.3.4")]
        public void BucketNameSuccessTest(string input)
        {
            Assert.True(_validator.TryValidateBucketName(input, out _));
        }

        [Theory]
        [InlineData("a")] //Too short
        [InlineData("a_b")] //Must not contain underscore
        [InlineData("aBc")] //Must not contain uppercase
        [InlineData("-asd")] //Must not start with hyphen
        public void BucketNameFailTest(string input)
        {
            Assert.False(_validator.TryValidateBucketName(input, out _));
        }

        [Theory]
        [InlineData("abcdeABCDE01234!-_.*'()", ObjectKeyValidationMode.SafeMode)]
        [InlineData("..", ObjectKeyValidationMode.SafeMode)]
        [InlineData("4my-organization", ObjectKeyValidationMode.SafeMode)]
        [InlineData("my.great_photos-2014/jan/myvacation.jpg", ObjectKeyValidationMode.SafeMode)]
        [InlineData("videos/2014/birthday/video1.wmv", ObjectKeyValidationMode.SafeMode)]
        [InlineData("this is a test?", ObjectKeyValidationMode.AsciiMode)]
        [InlineData("#[]<>", ObjectKeyValidationMode.ExtendedAsciiMode)]
        [InlineData("!\"#¤%&/()=?", ObjectKeyValidationMode.ExtendedAsciiMode)]
        public void ObjectKeySuccessTest(string input, ObjectKeyValidationMode mode)
        {
            Assert.True(_validator.TryValidateObjectKey(input, mode, out _));
        }

        [Theory]
        [InlineData(" ", ObjectKeyValidationMode.SafeMode)]
        [InlineData(".\\.", ObjectKeyValidationMode.SafeMode)]
        [InlineData("a/b[].jpg", ObjectKeyValidationMode.SafeMode)]
        public void ObjectKeyFailTest(string input, ObjectKeyValidationMode mode)
        {
            Assert.False(_validator.TryValidateObjectKey(input, mode, out _));
        }
    }
}