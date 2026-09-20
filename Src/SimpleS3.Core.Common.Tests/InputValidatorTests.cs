using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Tests.Code;
using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Core.Common.Tests;

public class InputValidatorTests
{
    private readonly TestInputValidator _validator = new TestInputValidator();

    [Theory]
    [InlineData(ObjectKeyValidationMode.Unrestricted)]
    [InlineData(ObjectKeyValidationMode.Default)]
    [InlineData(ObjectKeyValidationMode.DefaultStrict)]
    public void UnicodeObjectKeysAcceptValidPairsAndRejectMalformedUtf16(ObjectKeyValidationMode mode)
    {
        foreach (string key in new[] { "images/\U0001F600.png", "\U00010000\U0010FFFF", "text/自由" })
        {
            Assert.True(_validator.TryValidateObjectKey(key, mode, out ValidationStatus status, out _));
            Assert.Equal(ValidationStatus.Ok, status);
        }

        foreach (string key in new[] { "\uD800", "\uDC00", "\uD800a", "\uD800\uD800", "\uDC00\uD800", "\U0001F600\uD800" })
        {
            Assert.False(_validator.TryValidateObjectKey(key, mode, out ValidationStatus status, out _));
            Assert.Equal(ValidationStatus.WrongFormat, status);
        }
    }

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
        bool valid = _validator.TryValidateObjectKey(input, mode, out ValidationStatus status, out string? message);
        Assert.Equal(expectedStatus, status);
        Assert.Equal(expectedStatus == ValidationStatus.Ok, valid);
        Assert.Equal(valid, message == null);
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
        bool valid = _validator.TryValidateBucketName(input, mode, out ValidationStatus status, out string? message);
        Assert.Equal(expectedStatus, status);
        Assert.Equal(expectedStatus == ValidationStatus.Ok, valid);
        Assert.Equal(valid, message == null);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void NullNamesRespectValidationMode(bool bucket)
    {
        if (bucket)
        {
            foreach (BucketNameValidationMode mode in Enum.GetValues<BucketNameValidationMode>())
            {
                if (mode == BucketNameValidationMode.Unknown)
                    continue;
                bool valid = _validator.TryValidateBucketName(null, mode, out ValidationStatus status, out string? message);
                Assert.Equal(mode == BucketNameValidationMode.Disabled, valid);
                Assert.Equal(valid ? ValidationStatus.Ok : ValidationStatus.NullInput, status);
                Assert.Null(message);
            }
        }
        else
        {
            foreach (ObjectKeyValidationMode mode in Enum.GetValues<ObjectKeyValidationMode>())
            {
                if (mode == ObjectKeyValidationMode.Unknown)
                    continue;
                bool valid = _validator.TryValidateObjectKey(null, mode, out ValidationStatus status, out string? message);
                Assert.Equal(mode == ObjectKeyValidationMode.Disabled, valid);
                Assert.Equal(valid ? ValidationStatus.Ok : ValidationStatus.NullInput, status);
                Assert.Null(message);
            }
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid name/with spaces")]
    [InlineData("\uD800")]
    public void DisabledValidationAcceptsNonNullNames(string input)
    {
        Assert.True(_validator.TryValidateBucketName(input, BucketNameValidationMode.Disabled, out ValidationStatus bucketStatus, out string? bucketMessage));
        Assert.Equal(ValidationStatus.Ok, bucketStatus);
        Assert.Null(bucketMessage);
        Assert.True(_validator.TryValidateObjectKey(input, ObjectKeyValidationMode.Disabled, out ValidationStatus objectStatus, out string? objectMessage));
        Assert.Equal(ValidationStatus.Ok, objectStatus);
        Assert.Null(objectMessage);
    }

    [Theory]
    [InlineData(0, null)]
    [InlineData(0, "name")]
    [InlineData(255, null)]
    [InlineData(255, "name")]
    [InlineData(-1, null)]
    [InlineData(-1, "name")]
    public void InvalidModesRejectBeforeInspectingNames(int mode, string? input)
    {
        Assert.Throws<RequireException>(() =>
            _validator.TryValidateBucketName(input, (BucketNameValidationMode)mode, out _, out _));
        Assert.Throws<RequireException>(() =>
            _validator.TryValidateObjectKey(input, (ObjectKeyValidationMode)mode, out _, out _));
    }

    [Theory]
    [InlineData("abc-", false)]
    [InlineData("abc-.def", false)]
    [InlineData("abc.def-", false)]
    [InlineData("a-b.1", true)]
    [InlineData("a.1", true)]
    public void DnsLabelsRequireAlphanumericEnd(string input, bool expected)
    {
        bool valid = _validator.TryValidateBucketName(input, BucketNameValidationMode.DnsLabel, out ValidationStatus status, out string? message);
        Assert.Equal(expected, valid);
        Assert.Equal(expected ? ValidationStatus.Ok : ValidationStatus.WrongFormat, status);
        Assert.Equal(expected ? null : "-", message);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(63, true)]
    [InlineData(64, false)]
    public void DnsLabelLengthBoundaries(int length, bool expected)
    {
        string label = new string('a', length);

        foreach (string input in new[] { label, label + ".example", "example." + label + ".end" })
        {
            bool valid = _validator.TryValidateBucketName(input, BucketNameValidationMode.DnsLabel, out ValidationStatus status, out string? message);
            Assert.Equal(expected, valid);
            Assert.Equal(expected ? ValidationStatus.Ok : ValidationStatus.WrongLength, status);
            Assert.Equal(expected ? null : "1-63", message);
        }
    }
}