using Genbox.SimpleS3.Core.Abstracts.Enums;
using Xunit;

namespace Genbox.SimpleS3.Extensions.GoogleCloudStorage.Tests;

public class GoogleCloudStorageValidatorTests
{
    private readonly GoogleCloudStorageValidator _validator;

    public GoogleCloudStorageValidatorTests()
    {
        _validator = new GoogleCloudStorageValidator();
    }

    [Theory]
    [InlineData(null, ValidationStatus.NullInput)]
    [InlineData("my-travel-maps", ValidationStatus.Ok)]
    [InlineData("0f75d593-8e7b-4418-a5ba-cb2970f0b91e", ValidationStatus.Ok)]
    [InlineData("test.example.com", ValidationStatus.Ok)]
    [InlineData("goog-somebucket", ValidationStatus.ReservedName)] //contains goog
    [InlineData("agoogle", ValidationStatus.ReservedName)] //contains google
    [InlineData("_bucket_", ValidationStatus.WrongFormat)] //names must start with a-z or 0-9
    [InlineData("1", ValidationStatus.WrongLength)] //too short
    [InlineData("sometextsometextsometextsometextsometextsometextsometextsometext", ValidationStatus.WrongLength)] //too long
    [InlineData("My-Travel-Maps", ValidationStatus.WrongFormat)] //contains uppercase
    [InlineData("test bucket", ValidationStatus.WrongFormat)] //contains space
    public void TestBucketName(string name, ValidationStatus expectedStatus)
    {
        _validator.TryValidateBucketName(name, out ValidationStatus status, out _);

        Assert.Equal(expectedStatus, status);
    }

    [Theory]
    [InlineData(null, ValidationStatus.NullInput)]
    [InlineData("Kitten Videos", ValidationStatus.Ok)]
    [InlineData("users/beatrice/kitten.jpg", ValidationStatus.Ok)]
    [InlineData("自由.txt", ValidationStatus.Ok)]
    [InlineData("", ValidationStatus.WrongLength)] //too short
    [InlineData("test\n", ValidationStatus.WrongFormat)] //must not contain newline
    [InlineData("...", ValidationStatus.WrongFormat)] //must not be . or ...
    public void TestObjectKey(string name, ValidationStatus expectedStatus)
    {
        _validator.TryValidateObjectKey(name, ObjectKeyValidationMode.Unrestricted, out ValidationStatus status, out _);

        Assert.Equal(expectedStatus, status);
    }

    [Theory]
    [InlineData(null, ValidationStatus.NullInput)]
    [InlineData("GOOGTS7C7FUP3AIRVJTE2BCDKINBTES3HC2GY5CBFJDCQ2SYHV6A6XXVTJFSA", ValidationStatus.Ok)]
    [InlineData("GOOGTS7C7FUP3AIRVJTE2BCDKINB", ValidationStatus.WrongLength)]
    public void TestKeyId(string keyId, ValidationStatus expectedStatus)
    {
        _validator.TryValidateKeyId(keyId, out ValidationStatus status, out _);

        Assert.Equal(expectedStatus, status);
    }

    [Theory]
    [InlineData(null, ValidationStatus.NullInput)]
    [InlineData(new byte[] { 0, 0, }, ValidationStatus.WrongLength)]
    [InlineData(new byte[] { 98, 71, 111, 97, 43, 86, 55, 103, 47, 121, 113, 68, 88, 118, 75, 82, 113, 113, 43, 74, 84, 70, 110, 52, 117, 81, 90, 98, 80, 105, 81, 74, 111, 52, 112, 102, 57, 82, 122, 74 }, ValidationStatus.Ok)] //bGoa+V7g/yqDXvKRqq+JTFn4uQZbPiQJo4pf9RzJ
    public void TestAccessKey(byte[] key, ValidationStatus expectedStatus)
    {
        _validator.TryValidateAccessKey(key, out ValidationStatus status, out _);

        Assert.Equal(expectedStatus, status);
    }
}