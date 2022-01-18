using System;
using System.Linq;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Extensions.GoogleCloudStorage;

public class GoogleCloudStorageValidator : InputValidatorBase
{
    protected override bool TryValidateKeyIdInternal(string keyId, out ValidationStatus status, out string? message)
    {
        //https://cloud.google.com/storage/docs/authentication/hmackeys

        //Google service keys are 61. User keys are 24
        if (keyId.Length != 61 && keyId.Length != 24)
        {
            status = ValidationStatus.WrongLength;
            message = "24 / 61";
            return false;
        }

        foreach (char c in keyId)
        {
            if (CharHelper.InRange(c, 'a', 'z') || CharHelper.InRange(c, 'A', 'Z') || CharHelper.InRange(c, '0', '9'))
                continue;

            status = ValidationStatus.WrongFormat;
            message = c.ToString();
            return false;
        }

        status = ValidationStatus.Ok;
        message = null;
        return true;
    }

    protected override bool TryValidateAccessKeyInternal(byte[] accessKey, out ValidationStatus status, out string? message)
    {
        //https://cloud.google.com/storage/docs/authentication/hmackeys

        //Spec: A 40-character Base-64 encoded string that is linked to a specific access ID.
        if (accessKey.Length != 40)
        {
            status = ValidationStatus.WrongLength;
            message = "40";
            return false;
        }

        //Check that it is actually base64
        foreach (byte b in accessKey)
        {
            char c = (char)b;

            if (CharHelper.InRange(c, 'a', 'z') || CharHelper.InRange(c, 'A', 'Z') || CharHelper.InRange(c, '0', '9') || CharHelper.OneOf(c, '/', '+', '='))
                continue;

            status = ValidationStatus.WrongFormat;
            message = c.ToString();
            return false;
        }

        status = ValidationStatus.Ok;
        message = null;
        return true;
    }

    protected override bool TryValidateBucketNameInternal(string bucketName, out ValidationStatus status, out string? message)
    {
        //https://cloud.google.com/storage/docs/naming-buckets

        //Spec: Bucket names must contain 3-63 characters. Names containing dots can contain up to 222 characters, but each dot-separated component can be no longer than 63 characters.
        if (bucketName.Length is < 3 or > 222)
        {
            status = ValidationStatus.WrongLength;
            message = "3-63";
            return false;
        }

        int index = 0;

        for (int i = 0; i < bucketName.Length; i++)
        {
            if (bucketName[i] != '.')
                continue;

            int length = i - index;

            if (length > 63)
            {
                status = ValidationStatus.WrongLength;
                message = "1-63";
                return false;
            }

            index = i;
        }

        if (index == 0 && bucketName.Length > 63)
        {
            status = ValidationStatus.WrongLength;
            message = "1-63";
            return false;
        }

        //Spec: Bucket names must start and end with a number or letter.
        char start = bucketName[0];
        char end = bucketName.Last();

        if (!CharHelper.InRange(start, 'a', 'z') && !CharHelper.InRange(start, '0', '9'))
        {
            status = ValidationStatus.WrongFormat;
            message = start.ToString();
            return false;
        }

        if (!CharHelper.InRange(end, 'a', 'z') && !CharHelper.InRange(end, '0', '9'))
        {
            status = ValidationStatus.WrongFormat;
            message = end.ToString();
            return false;
        }

        //Spec: Bucket names must contain only lowercase letters, numbers, dashes (-), underscores (_), and dots (.). Spaces are not allowed. Names containing dots require verification.
        foreach (char c in bucketName)
        {
            if (CharHelper.InRange(c, 'a', 'z') || CharHelper.InRange(c, '0', '9') || CharHelper.OneOf(c, '-', '_', '.'))
                continue;

            status = ValidationStatus.WrongFormat;
            message = c.ToString();
            return false;
        }

        //Spec: Bucket names cannot begin with the "goog" prefix.
        //Spec: Bucket names cannot contain "google" or close misspellings, such as "g00gle".
        if (bucketName.StartsWith("goog", StringComparison.Ordinal) || bucketName.Contains("google") || bucketName.Contains("g00gle"))
        {
            status = ValidationStatus.ReservedName;
            message = bucketName;
            return false;
        }

        status = ValidationStatus.Ok;
        message = null;
        return true;
    }

    protected override bool TryValidateObjectKeyInternal(string objectKey, ObjectKeyValidationMode mode, out ValidationStatus status, out string? message)
    {
        //https://cloud.google.com/storage/docs/request-endpoints#encoding
        //https://cloud.google.com/storage/docs/naming-objects

        //Spec: Names can be pretty much any UTF-8 string up to 1024 bytes long
        if (objectKey.Length < 1 || Encoding.UTF8.GetByteCount(objectKey) > 1024)
        {
            status = ValidationStatus.WrongLength;
            message = "1-1024";
            return false;
        }

        //Spec: Objects cannot be named . or ...
        if (objectKey.Length == 1 && objectKey[0] == '.')
        {
            status = ValidationStatus.WrongFormat;
            message = ".";
            return false;
        }

        if (objectKey.Length == 3 && objectKey[0] == '.' && objectKey[1] == '.' && objectKey[2] == '.')
        {
            status = ValidationStatus.WrongFormat;
            message = "...";
            return false;
        }

        foreach (char c in objectKey)
        {
            //0xD800 to 0xDFFF are reserved code points in UTF-16. Since they will always be URL encoded to %EF%BF%BD (the � char) in UTF-8
            if (CharHelper.InRange(c, '\uD800', '\uDFFF'))
            {
                status = ValidationStatus.WrongFormat;
                message = c.ToString();
                return false;
            }
            //Spec: Object names cannot contain Carriage Return or Line Feed characters.
            if (CharHelper.OneOf(c, '\r', '\n'))
            {
                status = ValidationStatus.WrongFormat;
                message = c.ToString();
                return false;
            }

            if (CharHelper.InRange(c, 'a', 'z') || CharHelper.InRange(c, 'A', 'Z') || CharHelper.InRange(c, '0', '9'))
                continue;

            if (CharHelper.OneOf(c, '!', '#', '$', '&', '\'', '(', ')', '*', '+', ',', '/', ':', ';', '=', '?', '@', '[', ']'))
                continue;

            if (mode == ObjectKeyValidationMode.Unrestricted)
                continue;

            status = ValidationStatus.WrongFormat;
            message = c.ToString();
            return false;
        }

        //Spec: Object names cannot start with .well-known/acme-challenge/.
        if (objectKey.StartsWith(".well-known/acme-challenge/", StringComparison.OrdinalIgnoreCase))
        {
            status = ValidationStatus.WrongFormat;
            message = ".well-known/acme-challenge/";
            return false;
        }

        status = ValidationStatus.Ok;
        message = null;
        return true;
    }
}