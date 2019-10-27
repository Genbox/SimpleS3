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
        [InlineData("abcdeABCDE01234!-_.*'()", Level.Level3)]
        [InlineData("..", Level.Level3)]
        [InlineData("4my-organization", Level.Level3)]
        [InlineData("my.great_photos-2014/jan/myvacation.jpg", Level.Level3)]
        [InlineData("videos/2014/birthday/video1.wmv", Level.Level3)]
        [InlineData("this is a test?", Level.Level2)]
        [InlineData("#[]<>", Level.Level1)]
        [InlineData("!\"#¤%&/()=?", Level.Level0)]
        public void ObjectKeySuccessTest(string input, Level mode)
        {
            Assert.True(InputValidator.TryValidateObjectKey(input, mode, out _));
        }

        [Theory]
        [InlineData(" ", Level.Level3)]
        [InlineData(".\\.", Level.Level3)]
        [InlineData("a/b[].jpg", Level.Level3)]
        public void ObjectKeyFailTest(string input, Level mode)
        {
            Assert.False(InputValidator.TryValidateObjectKey(input, mode, out _));
        }
    }
}