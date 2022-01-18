using System.Globalization;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Internals.Misc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Authentication;

internal class SigningKeyBuilder : ISigningKeyBuilder
{
    private readonly ILogger<SigningKeyBuilder> _logger;
    private readonly IOptions<SimpleS3Config> _options;
    private readonly IAccessKeyProtector? _protector;
    private readonly byte[] _serviceBytes;
    private readonly byte[] _regionBytes;
    private readonly byte[] _requestBytes;
    private readonly byte[] _schemeBytes;
    private readonly byte[] _dateBytes;

    public SigningKeyBuilder(IOptions<SimpleS3Config> options, ILogger<SigningKeyBuilder> logger, IAccessKeyProtector? protector = null)
    {
        _options = options;
        _logger = logger;
        _protector = protector;

        //Cache ac couple of things to make signing faster
        _serviceBytes = Encoding.UTF8.GetBytes("s3");
        _regionBytes = Encoding.UTF8.GetBytes(_options.Value.RegionCode);
        _requestBytes = Encoding.UTF8.GetBytes("aws4_request");
        _schemeBytes = Encoding.UTF8.GetBytes(SigningConstants.Scheme);
        _dateBytes = new byte[8];
    }

    public byte[] CreateSigningKey(DateTimeOffset dateTime)
    {
        _logger.LogTrace("Creating key created on {DateTime}", dateTime);

        //https://docs.aws.amazon.com/AmazonS3/latest/API/sig-v4-header-based-auth.html
        //Documentation says the key is valid for 7 days, but tests shows that is not true.
        string date = dateTime.ToString(DateTimeFormats.Iso8601Date, DateTimeFormatInfo.InvariantInfo);
        Encoding.UTF8.GetBytes(date, 0, date.Length, _dateBytes, 0);

        byte[] accessKey = KeyHelper.UnprotectKey(_options.Value.Credentials.SecretKey, _protector);

        byte[] key = new byte[_schemeBytes.Length + accessKey.Length];
        Array.Copy(_schemeBytes, 0, key, 0, _schemeBytes.Length);
        Array.Copy(accessKey, 0, key, _schemeBytes.Length, accessKey.Length);

        //Security: If the access key is protected, delete it from memory
        if (_protector != null)
            Array.Clear(accessKey, 0, accessKey.Length);

        byte[] hashDate = CryptoHelper.HmacSign(_dateBytes, key);

        //Security: clear key material
        Array.Clear(key, 0, key.Length);

        byte[] hashRegion = CryptoHelper.HmacSign(_regionBytes, hashDate);
        byte[] hashService = CryptoHelper.HmacSign(_serviceBytes, hashRegion);
        byte[] signingKey = CryptoHelper.HmacSign(_requestBytes, hashService);

        _logger.LogDebug("Signing key created: {SigningKey}", signingKey);

        return signingKey;
    }
}