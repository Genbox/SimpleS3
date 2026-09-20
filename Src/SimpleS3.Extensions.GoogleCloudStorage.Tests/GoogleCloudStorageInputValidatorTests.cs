using Genbox.SimpleS3.Core.Abstracts.Enums;

namespace Genbox.SimpleS3.Extensions.GoogleCloudStorage.Tests;

public class GoogleCloudStorageInputValidatorTests
{
    private readonly GoogleCloudStorageInputValidator _validator = new GoogleCloudStorageInputValidator();

    [Theory]
    [InlineData(null, ValidationStatus.NullInput)]
    [InlineData("GOOGTS7C7FUP3AIRVJTE2BCDKINBTES3HC2GY5CBFJDCQ2SYHV6A6XXVTJFSA", ValidationStatus.Ok)]
    [InlineData("GOOGTS7C7FUP3AIRVJTE2BCDKINB", ValidationStatus.WrongLength)]
    public void TryValidateKeyIdTest(string? keyId, ValidationStatus expectedStatus)
    {
        bool valid = _validator.TryValidateKeyId(keyId, out ValidationStatus status, out _);
        Assert.Equal(expectedStatus, status);
        Assert.Equal(expectedStatus == ValidationStatus.Ok, valid);
    }

    [Theory]
    [InlineData(null, ValidationStatus.NullInput)]
    [InlineData(new byte[] { 0, 0 }, ValidationStatus.WrongLength)]
    [InlineData(new byte[] { 98, 71, 111, 97, 43, 86, 55, 103, 47, 121, 113, 68, 88, 118, 75, 82, 113, 113, 43, 74, 84, 70, 110, 52, 117, 81, 90, 98, 80, 105, 81, 74, 111, 52, 112, 102, 57, 82, 122, 74 }, ValidationStatus.Ok)] //bGoa+V7g/yqDXvKRqq+JTFn4uQZbPiQJo4pf9RzJ
    public void TryValidateAccessKeyTest(byte[]? key, ValidationStatus expectedStatus)
    {
        byte[]? original = key?.ToArray();
        bool valid = _validator.TryValidateAccessKey(key, out ValidationStatus status, out _);
        Assert.Equal(expectedStatus, status);
        Assert.Equal(expectedStatus == ValidationStatus.Ok, valid);
        Assert.Equal(original, key);
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
    public void TryValidateBucketNameTest(string? name, ValidationStatus expectedStatus)
    {
        bool valid = _validator.TryValidateBucketName(name, BucketNameValidationMode.Default, out ValidationStatus status, out _);
        Assert.Equal(expectedStatus, status);
        Assert.Equal(expectedStatus == ValidationStatus.Ok, valid);
    }

    [Theory]
    [InlineData(null, ValidationStatus.NullInput)]
    [InlineData("Kitten Videos", ValidationStatus.Ok)]
    [InlineData("users/beatrice\\kitten.jpg", ValidationStatus.Ok)]
    [InlineData("", ValidationStatus.WrongLength)] //too short
    [InlineData("test\n", ValidationStatus.WrongFormat)] //must not contain newline
    [InlineData(".", ValidationStatus.WrongFormat)] //Single and double dot names are reserved
    [InlineData("..", ValidationStatus.WrongFormat)]
    [InlineData("...", ValidationStatus.Ok)]
    [InlineData("大三.txt", ValidationStatus.Ok)] //Test we can use UTF8 chars
    public void TryValidateObjectKeyTest(string? name, ValidationStatus expectedStatus)
    {
        bool valid = _validator.TryValidateObjectKey(name, ObjectKeyValidationMode.Default, out ValidationStatus status, out _);
        Assert.Equal(expectedStatus, status);
        Assert.Equal(expectedStatus == ValidationStatus.Ok, valid);
    }

    [Theory]
    [InlineData(63, false, true)]
    [InlineData(64, false, false)]
    [InlineData(64, true, true)]
    [InlineData(222, true, true)]
    [InlineData(223, true, false)]
    public void BucketTotalLengthRespectsDottedNames(int length, bool dotted, bool expected)
    {
        char[] chars = new string('a', length).ToCharArray();

        if (dotted)
        {
            for (int i = 60; i < length; i += 61)
                chars[i] = '.';
        }

        bool valid = _validator.TryValidateBucketName(new string(chars), BucketNameValidationMode.Default, out ValidationStatus status, out _);
        Assert.Equal(expected, valid);
        Assert.Equal(expected ? ValidationStatus.Ok : ValidationStatus.WrongLength, status);
    }

    [Theory]
    [InlineData(63, true)]
    [InlineData(64, false)]
    public void DottedBucketComponentLengthIsLimited(int length, bool expected)
    {
        string label = new string('a', length);

        foreach (string name in new[] { label + ".example", "example." + label })
        {
            bool valid = _validator.TryValidateBucketName(name, BucketNameValidationMode.Default, out ValidationStatus status, out _);
            Assert.Equal(expected, valid);
            Assert.Equal(expected ? ValidationStatus.Ok : ValidationStatus.WrongLength, status);
        }
    }

    [Theory]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa..com")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.-dns.com")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.dns-.com")]
    [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.dns_name.com")]
    public void LongDottedBucketNamesRequireValidDnsComponents(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        Assert.True(name.Length > 63);
        Assert.False(_validator.TryValidateBucketName(name, BucketNameValidationMode.Default, out ValidationStatus status, out _));
        Assert.NotEqual(ValidationStatus.Ok, status);
    }

    [Theory]
    [InlineData("192.168.5.4", false)]
    [InlineData("0.0.0.0", false)]
    [InlineData("255.255.255.255", false)]
    [InlineData("123", true)]
    [InlineData("1.2.3", true)]
    [InlineData("1.2.3.example", true)]
    [InlineData("example.1.2.3.4", true)]
    public void BucketNamesRejectDottedDecimalAddresses(string name, bool expected)
    {
        bool valid = _validator.TryValidateBucketName(name, BucketNameValidationMode.Default, out ValidationStatus status, out string? message);
        Assert.Equal(expected, valid);
        Assert.Equal(expected ? ValidationStatus.Ok : ValidationStatus.WrongFormat, status);
        Assert.Equal(expected ? null : name, message);
    }

    [Theory]
    [InlineData("a", 1024, true)]
    [InlineData("a", 1025, false)]
    [InlineData("é", 512, true)]
    [InlineData("é", 513, false)]
    [InlineData("😀", 256, true)]
    [InlineData("😀", 257, false)]
    public void ObjectNameLimitCountsUtf8Bytes(string unit, int count, bool expected)
    {
        string name = string.Concat(Enumerable.Repeat(unit, count));

        foreach (ObjectKeyValidationMode mode in new[] { ObjectKeyValidationMode.Default, ObjectKeyValidationMode.DefaultStrict })
        {
            bool valid = _validator.TryValidateObjectKey(name, mode, out ValidationStatus status, out string? message);
            Assert.Equal(expected, valid);
            Assert.Equal(expected ? ValidationStatus.Ok : ValidationStatus.WrongLength, status);
            Assert.Equal(expected ? null : "1-1024", message);
        }
    }

    [Theory]
    [InlineData('#')]
    [InlineData('[')]
    [InlineData(']')]
    [InlineData('*')]
    [InlineData('?')]
    public void StrictModeRejectsCliSensitiveCharacters(char character)
    {
        string name = "folder/name" + character + "suffix";
        Assert.True(_validator.TryValidateObjectKey(name, ObjectKeyValidationMode.Default, out ValidationStatus normalStatus, out string? normalMessage));
        Assert.Equal(ValidationStatus.Ok, normalStatus);
        Assert.Null(normalMessage);
        Assert.False(_validator.TryValidateObjectKey(name, ObjectKeyValidationMode.DefaultStrict, out ValidationStatus strictStatus, out string? strictMessage));
        Assert.Equal(ValidationStatus.WrongFormat, strictStatus);
        Assert.Equal(character.ToString(), strictMessage);
    }

    [Theory]
    [InlineData(".well-known/acme-challenge/audit", false)]
    [InlineData(".WELL-KNOWN/ACME-CHALLENGE/audit", true)]
    [InlineData("nested/.well-known/acme-challenge/audit", true)]
    public void ReservedAcmePrefixUsesExactCaseAndPosition(string key, bool expected)
    {
        foreach (ObjectKeyValidationMode mode in new[] { ObjectKeyValidationMode.Default, ObjectKeyValidationMode.DefaultStrict })
        {
            Assert.Equal(expected, _validator.TryValidateObjectKey(key, mode, out ValidationStatus status, out string? message));
            Assert.Equal(expected ? ValidationStatus.Ok : ValidationStatus.WrongFormat, status);
            Assert.Equal(expected ? null : ".well-known/acme-challenge/", message);
        }
    }
}