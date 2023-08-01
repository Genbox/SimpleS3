using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Extensions.GenericS3.Tests;

public class GenericS3InputValidatorTests
{
    private readonly GenericS3InputValidator _validator;

    public GenericS3InputValidatorTests()
    {
        _validator = new GenericS3InputValidator();
    }

    [Fact]
    public void TryValidateKeyIdTest()
    {
        _validator.TryValidateKeyId("<something1random>", out ValidationStatus status, out _);
        Assert.Equal(ValidationStatus.Ok, status);
    }

    [Fact]
    public void TryValidateAccessKeyTest()
    {
        _validator.TryValidateAccessKey(new byte[] { 1, 2, 3 }, out ValidationStatus status, out _);
        Assert.Equal(ValidationStatus.Ok, status);
    }

    [Fact]
    public void TryValidateBucketNameTest()
    {
        _validator.TryValidateBucketName("<something1random>", BucketNameValidationMode.Default, out ValidationStatus status, out _);
        Assert.Equal(ValidationStatus.Ok, status);
    }

    [Fact]
    public void TryValidateObjectKeyTest()
    {
        _validator.TryValidateObjectKey("<something1random>", ObjectKeyValidationMode.Default, out ValidationStatus status, out _);
        Assert.Equal(ValidationStatus.Ok, status);
    }
}