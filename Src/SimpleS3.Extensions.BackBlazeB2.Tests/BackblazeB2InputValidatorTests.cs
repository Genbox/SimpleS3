using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Extensions.BackBlazeB2.Tests;

public class BackblazeB2InputValidatorTests
{
    private readonly BackblazeB2InputValidator _validator = new BackblazeB2InputValidator();

    [Theory]
    [InlineData(null, ValidationStatus.NullInput)]
    [InlineData("61a951f6abb8", ValidationStatus.Ok)] //12 length hex should be ok
    [InlineData("00261a951f6abb80000000001", ValidationStatus.Ok)] //25 length hex should be ok
    [InlineData("ghighighighi", ValidationStatus.WrongFormat)] //only a-f and 0-9 allowed
    [InlineData("1234567890AB", ValidationStatus.WrongFormat)] //uppercase not allowed
    [InlineData("61a951f6ab", ValidationStatus.WrongLength)] //too short
    public void TryValidateKeyIdTest(string? keyId, ValidationStatus expectedStatus)
    {
        _validator.TryValidateKeyId(keyId, out ValidationStatus status, out _);
        Assert.Equal(expectedStatus, status);
    }

    [Theory]
    [InlineData(null, ValidationStatus.NullInput)]
    [InlineData(new byte[] { 0, 0 }, ValidationStatus.WrongLength)]
    [InlineData(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, ValidationStatus.Ok)]
    public void TryValidateAccessKeyTest(byte[]? key, ValidationStatus expectedStatus)
    {
        _validator.TryValidateAccessKey(key, out ValidationStatus status, out _);
        Assert.Equal(expectedStatus, status);
    }

    [Theory]
    [InlineData(null, ValidationStatus.NullInput)]
    [InlineData("myBucket", ValidationStatus.Ok)]
    [InlineData("backblaze-images", ValidationStatus.Ok)]
    [InlineData("bucket-74358734", ValidationStatus.Ok)]
    [InlineData("b2-somebucket", ValidationStatus.ReservedName)]
    [InlineData("a", ValidationStatus.WrongLength)]
    [InlineData("!ABCDEF!", ValidationStatus.WrongFormat)]
    [InlineData("contains space", ValidationStatus.WrongFormat)]
    public void TryValidateBucketNameTest(string? name, ValidationStatus expectedStatus)
    {
        _validator.TryValidateBucketName(name, BucketNameValidationMode.Default, out ValidationStatus status, out _);
        Assert.Equal(expectedStatus, status);
    }

    [Theory]
    [InlineData(null, ValidationStatus.NullInput)]
    [InlineData("Kitten Videos", ValidationStatus.Ok)]
    [InlineData("users/beatrice/kitten.jpg", ValidationStatus.Ok)]
    [InlineData("", ValidationStatus.WrongLength)]
    [InlineData("\0", ValidationStatus.WrongFormat)]
    [InlineData("大三.txt", ValidationStatus.Ok)] //Test we can use UTF8 chars
    public void TryValidateObjectKeyTest(string? name, ValidationStatus expectedStatus)
    {
        _validator.TryValidateObjectKey(name, ObjectKeyValidationMode.Default, out ValidationStatus status, out _);
        Assert.Equal(expectedStatus, status);
    }
}