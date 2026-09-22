using System.Globalization;
using System.Security.Cryptography;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Authentication;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Network.Requests;
using Genbox.SimpleS3.Core.Network.Requests.Interfaces;

namespace Genbox.SimpleS3.Core.Internals.Helpers;

internal static class ChecksumHelper
{
    private static readonly (ChecksumAlgorithm Algorithm, string Header)[] _algorithms =
    [
        (ChecksumAlgorithm.Crc32, AmzHeaders.XAmzChecksumCrc32),
        (ChecksumAlgorithm.Crc32C, AmzHeaders.XAmzChecksumCrc32C),
        (ChecksumAlgorithm.Crc64Nvme, AmzHeaders.XAmzChecksumCrc64Nvme),
        (ChecksumAlgorithm.Sha1, AmzHeaders.XAmzChecksumSha1),
        (ChecksumAlgorithm.Sha256, AmzHeaders.XAmzChecksumSha256),
        (ChecksumAlgorithm.Sha512, AmzHeaders.XAmzChecksumSha512),
        (ChecksumAlgorithm.Md5, AmzHeaders.XAmzChecksumMd5),
        (ChecksumAlgorithm.XxHash64, AmzHeaders.XAmzChecksumXxHash64),
        (ChecksumAlgorithm.XxHash3, AmzHeaders.XAmzChecksumXxHash3),
        (ChecksumAlgorithm.XxHash128, AmzHeaders.XAmzChecksumXxHash128)
    ];

    public static (ChecksumAlgorithm Algorithm, byte[]? Checksum) ReadHeaders(IDictionary<string, string> headers) => ReadHeaders(headers, out _);

    public static (ChecksumAlgorithm Algorithm, byte[]? Checksum) ReadHeaders(IDictionary<string, string> headers, out bool composite)
    {
        composite = false;

        foreach ((ChecksumAlgorithm Algorithm, string Header) algorithm in _algorithms)
        {
            if (headers.TryGetHeader(algorithm.Header, out string? value))
                return (algorithm.Algorithm, Decode(value, out composite));
        }

        return (ChecksumAlgorithm.Unknown, null);
    }

    public static ChecksumAlgorithm GetXmlAlgorithm(string elementName) => elementName switch
    {
        "ChecksumCRC32" => ChecksumAlgorithm.Crc32,
        "ChecksumCRC32C" => ChecksumAlgorithm.Crc32C,
        "ChecksumCRC64NVME" => ChecksumAlgorithm.Crc64Nvme,
        "ChecksumSHA1" => ChecksumAlgorithm.Sha1,
        "ChecksumSHA256" => ChecksumAlgorithm.Sha256,
        "ChecksumSHA512" => ChecksumAlgorithm.Sha512,
        "ChecksumMD5" => ChecksumAlgorithm.Md5,
        "ChecksumXXHASH64" => ChecksumAlgorithm.XxHash64,
        "ChecksumXXHASH3" => ChecksumAlgorithm.XxHash3,
        "ChecksumXXHASH128" => ChecksumAlgorithm.XxHash128,
        _ => ChecksumAlgorithm.Unknown
    };

    public static byte[] ParseHash(string value) => Decode(value, out _);

    public static void ApplyRequest(BaseRequest request, IHasChecksumProperties checksum, byte[] body)
    {
        if (checksum.ChecksumAlgorithm == ChecksumAlgorithm.Unknown)
            return;

        byte[] value;

        if (checksum.Checksum == null)
        {
            using HashAlgorithm algorithm = ChecksumHashAlgorithm.Create(checksum.ChecksumAlgorithm);
            value = algorithm.ComputeHash(body);
        }
        else
            value = checksum.Checksum;

        request.SetHeader(AmzHeaders.XAmzSdkCheckumAlgorithm, checksum.ChecksumAlgorithm.GetDisplayName());
        request.SetHeader(AmzHeaders.XAmzChecksum + checksum.ChecksumAlgorithm.ToString().ToLowerInvariant(), Convert.ToBase64String(value));
    }

    public static byte[] Decode(string value, out bool composite)
    {
        int separator = value.LastIndexOf('-');
        composite = separator >= 0;
        if (!composite)
            return Convert.FromBase64String(value);
        if (separator == 0 || !int.TryParse(value.Substring(separator + 1), NumberStyles.None, CultureInfo.InvariantCulture, out int parts) || parts <= 0)
            throw new FormatException("Invalid multipart checksum part count.");
        return Convert.FromBase64String(value.Substring(0, separator));
    }
}