using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Tests.Code;
using Xunit;

namespace Genbox.SimpleS3.Core.Common.Tests;

public class InputValidatorTests
{
    private readonly TestInputValidator _validator = new TestInputValidator();

    [Theory]
    [InlineData("abcdeABCDE01234", ObjectKeyValidationMode.SafeMode, ValidationStatus.Ok)]
    [InlineData("!#$", ObjectKeyValidationMode.SafeMode, ValidationStatus.WrongFormat)]
    [InlineData("!\"#%&/()=?", ObjectKeyValidationMode.AsciiMode, ValidationStatus.Ok)]
    [InlineData("æøå½", ObjectKeyValidationMode.AsciiMode, ValidationStatus.WrongFormat)]
    [InlineData("æøåÆØÅ½", ObjectKeyValidationMode.ExtendedAsciiMode, ValidationStatus.Ok)]
    [InlineData("\u0300", ObjectKeyValidationMode.ExtendedAsciiMode, ValidationStatus.WrongFormat)]
    [InlineData("自由", ObjectKeyValidationMode.Unrestricted, ValidationStatus.Ok)] //Anything should be allowed when it is unrestricted
    public void TryValidateObjectKeySuccessTest(string input, ObjectKeyValidationMode mode, ValidationStatus expectedStatus)
    {
        _validator.TryValidateObjectKey(input, mode, out ValidationStatus status, out _);
        Assert.Equal(expectedStatus, status);
    }

    [Theory]
    [InlineData("abc.def.123", BucketNameValidationMode.DnsLabel, ValidationStatus.Ok)] //Normal DNS
    [InlineData("ab-c.def.123", BucketNameValidationMode.DnsLabel, ValidationStatus.Ok)] //It is valid to contain dash
    [InlineData(".", BucketNameValidationMode.DnsLabel, ValidationStatus.WrongLength)] //Root label is not valid
    [InlineData("b..a", BucketNameValidationMode.DnsLabel, ValidationStatus.WrongLength)] //Empty label is not valid
    [InlineData("-b..a", BucketNameValidationMode.DnsLabel, ValidationStatus.WrongFormat)] //Can't start with a dash
    [InlineData("自由", BucketNameValidationMode.Unrestricted, ValidationStatus.Ok)] //Anything should be allowed when it is unrestricted
    public void TryValidateBucketNameSuccessTest(string input, BucketNameValidationMode mode, ValidationStatus expectedStatus)
    {
        _validator.TryValidateBucketName(input, mode, out ValidationStatus status, out _);
        Assert.Equal(expectedStatus, status);
    }
}