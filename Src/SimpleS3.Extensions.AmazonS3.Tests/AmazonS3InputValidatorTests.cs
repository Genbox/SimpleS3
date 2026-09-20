using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Extensions.AmazonS3.Tests;

public class AmazonS3InputValidatorTests
{
    private readonly AmazonS3InputValidator _validator = new AmazonS3InputValidator();

    [Theory]
    [InlineData(ObjectKeyValidationMode.Default)]
    [InlineData(ObjectKeyValidationMode.DefaultStrict)]
    public void UnicodeObjectKeysUseUtf8ByteLimits(ObjectKeyValidationMode mode)
    {
        string key = string.Concat(Enumerable.Repeat("\U0001F600", 256));

        Assert.True(_validator.TryValidateObjectKey(key, mode, out ValidationStatus valid, out _));
        Assert.Equal(ValidationStatus.Ok, valid);
        Assert.False(_validator.TryValidateObjectKey(key + "a", mode, out ValidationStatus tooLong, out _));
        Assert.Equal(ValidationStatus.WrongLength, tooLong);
    }

    [Theory]
    [InlineData(null, ValidationStatus.NullInput)]
    [InlineData("AKIAIOSFODNN7EXAMPLE", ValidationStatus.Ok)] //Normal key id
    [InlineData("1AKIAIOSFODNN7EXAMPLE", ValidationStatus.WrongLength)] //Too long
    [InlineData("aKIAIOSFODNN7EXAMPLE", ValidationStatus.WrongFormat)] //It must be uppercase
    public void TryValidateKeyIdTest(string? keyId, ValidationStatus expectedStatus)
    {
        bool valid = _validator.TryValidateKeyId(keyId, out ValidationStatus status, out _);
        Assert.Equal(expectedStatus, status);
        Assert.Equal(expectedStatus == ValidationStatus.Ok, valid);
    }

    [Theory]
    [InlineData(null, ValidationStatus.NullInput)]
    [InlineData(new byte[] { 0, 0 }, ValidationStatus.WrongLength)]
    [InlineData(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, ValidationStatus.Ok)]
    public void TryValidateAccessKeyTest(byte[]? key, ValidationStatus expectedStatus)
    {
        byte[]? original = key?.ToArray();
        bool valid = _validator.TryValidateAccessKey(key, out ValidationStatus status, out _);
        Assert.Equal(expectedStatus, status);
        Assert.Equal(expectedStatus == ValidationStatus.Ok, valid);
        Assert.Equal(original, key);
    }

    [Theory]
    [InlineData(null, ValidationStatus.NullInput)] //Null is not allowed
    [InlineData("abc", ValidationStatus.Ok)]
    [InlineData("a.b.c.d", ValidationStatus.Ok)]
    [InlineData("my-bucket-name", ValidationStatus.Ok)]
    [InlineData("", ValidationStatus.WrongLength)] //Empty is not allowed
    [InlineData("a", ValidationStatus.WrongLength)] //Must be between 3 and 63 characters long
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", ValidationStatus.WrongLength)] //Bucket names must be between 3 and 63 characters long
    [InlineData("a_b", ValidationStatus.WrongFormat)] //Must consist only of lowercase letters, numbers, dots (.), and hyphens (-)
    [InlineData("aBc", ValidationStatus.WrongFormat)] //Must consist only of lowercase letters, numbers, dots (.), and hyphens (-)
    [InlineData("-asd", ValidationStatus.WrongFormat)] //Must consist only of lowercase letters, numbers, dots (.), and hyphens (-)
    [InlineData("^bucket", ValidationStatus.WrongFormat)] //Must must begin and end with a letter or number
    [InlineData("bucket^", ValidationStatus.WrongFormat)] //Must must begin and end with a letter or number
    [InlineData("127.0.0.1", ValidationStatus.WrongFormat)] //Must not be formatted as an IP address (for example, 192.168.5.4)
    [InlineData("xn--punyhulk", ValidationStatus.WrongFormat)] //Must not start with the prefix xn--
    [InlineData("bucket-s3alias", ValidationStatus.Ok)] //Alias references are allowed; creation restrictions require request context
    public void TryValidateBucketNameTest(string? input, ValidationStatus expectedStatus)
    {
        bool valid = _validator.TryValidateBucketName(input, BucketNameValidationMode.Default, out ValidationStatus status, out _);
        Assert.Equal(expectedStatus, status);
        Assert.Equal(expectedStatus == ValidationStatus.Ok, valid);
    }

    [Theory]
    [InlineData(null, ValidationStatus.NullInput)]
    [InlineData("my-Organization", ValidationStatus.Ok)] //Test dash and casing
    [InlineData("Private/taxdocument.pdf", ValidationStatus.Ok)] //Test if slash is valid
    [InlineData("大三.txt", ValidationStatus.Ok)] //Test we can use UTF8 chars
    public void TryValidateObjectKeyTest(string? input, ValidationStatus expectedStatus)
    {
        bool valid = _validator.TryValidateObjectKey(input, ObjectKeyValidationMode.Default, out ValidationStatus status, out _);
        Assert.Equal(expectedStatus, status);
        Assert.Equal(expectedStatus == ValidationStatus.Ok, valid);
    }

    [Theory]
    [InlineData("example..com", false)]
    [InlineData("a...b", false)]
    [InlineData("example.com", true)]
    public void AdjacentBucketPeriodsAreRejected(string name, bool expected)
    {
        bool valid = _validator.TryValidateBucketName(name, BucketNameValidationMode.Default, out ValidationStatus status, out string? message);
        Assert.Equal(expected, valid);
        Assert.Equal(expected ? ValidationStatus.Ok : ValidationStatus.WrongFormat, status);
        Assert.Equal(expected ? null : "..", message);
    }
}