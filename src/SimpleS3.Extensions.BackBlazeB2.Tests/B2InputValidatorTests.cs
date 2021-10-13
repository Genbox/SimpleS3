using Genbox.SimpleS3.Core.Abstracts.Enums;
using Xunit;

namespace Genbox.SimpleS3.Extensions.BackBlazeB2.Tests
{
    public class B2InputValidatorTests
    {
        private readonly BackblazeB2InputValidator _validator;

        public B2InputValidatorTests()
        {
            _validator = new BackblazeB2InputValidator();
        }

        [Theory]
        [InlineData(null, ValidationStatus.NullInput)]
        [InlineData("myBucket", ValidationStatus.Ok)]
        [InlineData("backblaze-images", ValidationStatus.Ok)]
        [InlineData("bucket-74358734", ValidationStatus.Ok)]
        [InlineData("b2-somebucket", ValidationStatus.ReservedName)]
        [InlineData("a", ValidationStatus.WrongLength)]
        [InlineData("!ABCDEF!", ValidationStatus.WrongFormat)]
        public void TestBucketName(string name, ValidationStatus expectedStatus)
        {
            _validator.TryValidateBucketName(name, out ValidationStatus status);

            Assert.Equal(expectedStatus, status);
        }

        [Theory]
        [InlineData(null, ValidationStatus.NullInput)]
        [InlineData("Kitten Videos", ValidationStatus.Ok)]
        [InlineData("users/beatrice/kitten.jpg", ValidationStatus.Ok)]
        [InlineData("自由.txt", ValidationStatus.Ok)]
        [InlineData("", ValidationStatus.WrongLength)]
        [InlineData("\0", ValidationStatus.WrongFormat)]
        public void TestObjectKey(string name, ValidationStatus expectedStatus)
        {
            _validator.TryValidateObjectKey(name, ObjectKeyValidationMode.Unrestricted, out ValidationStatus status);

            Assert.Equal(expectedStatus, status);
        }

        [Theory]
        [InlineData(null, ValidationStatus.NullInput)]
        [InlineData("61a951f6abb8", ValidationStatus.Ok)]
        [InlineData("00261a951f6abb80000000001", ValidationStatus.Ok)]
        public void TestKeyId(string keyId, ValidationStatus expectedStatus)
        {
            _validator.TryValidateKeyId(keyId, out ValidationStatus status);

            Assert.Equal(expectedStatus, status);
        }

        [Theory]
        [InlineData(null, ValidationStatus.NullInput)]
        [InlineData(new byte[] { 0, 0, }, ValidationStatus.WrongLength)]
        [InlineData(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, ValidationStatus.Ok)]
        public void TestAccessKey(byte[] key, ValidationStatus expectedStatus)
        {
            _validator.TryValidateAccessKey(key, out ValidationStatus status);

            Assert.Equal(expectedStatus, status);
        }
    }
}