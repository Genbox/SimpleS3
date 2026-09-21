using System.Globalization;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Misc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Authentication;

internal sealed partial class SigningKeyBuilder : ISigningKeyBuilder
{
    private readonly SimpleS3Config _config;
    private readonly IAccessKeyProtector? _protector;
    private readonly byte[] _requestBytes;
    private readonly byte[] _schemeBytes;

    public SigningKeyBuilder(IOptions<SimpleS3Config> options, ILogger<SigningKeyBuilder> logger, IAccessKeyProtector? protector = null)
    {
        _config = options.Value;
        _logger = logger;
        _protector = protector;

        // Cache fixed signing components. RegionCode remains mutable.
        _requestBytes = "aws4_request"u8.ToArray();
        _schemeBytes = "AWS4"u8.ToArray();
    }

    public byte[] CreateSigningKey(DateTimeOffset dateTime, string service)
    {
        Validator.RequireNotNull(_config.Credentials, "If we get to this point, we expect to have valid credentials");
        CreatingSigningKey(dateTime);

        //https://docs.aws.amazon.com/AmazonS3/latest/API/sig-v4-header-based-auth.html
        //Documentation says the key is valid for 7 days, but tests shows that is not true.
        string date = dateTime.ToString(DateTimeFormats.Iso8601Date, DateTimeFormatInfo.InvariantInfo);
        byte[] dateBytes = Encoding.UTF8.GetBytes(date);

        byte[] accessKey = KeyHelper.UnprotectKey(_config.Credentials.SecretKey, _protector);

        byte[] key = new byte[_schemeBytes.Length + accessKey.Length];
        Array.Copy(_schemeBytes, 0, key, 0, _schemeBytes.Length);
        Array.Copy(accessKey, 0, key, _schemeBytes.Length, accessKey.Length);

        //Security: If the access key is protected, delete it from memory
        if (_protector != null)
            Array.Clear(accessKey, 0, accessKey.Length);

        byte[] hashDate = CryptoHelper.HmacSign(dateBytes, key);

        //Security: clear key material
        Array.Clear(dateBytes, 0, dateBytes.Length);
        Array.Clear(key, 0, key.Length);

        byte[] hashRegion = CryptoHelper.HmacSign(Encoding.UTF8.GetBytes(_config.RegionCode), hashDate);
        byte[] hashService = CryptoHelper.HmacSign(Encoding.UTF8.GetBytes(service), hashRegion);
        byte[] signingKey = CryptoHelper.HmacSign(_requestBytes, hashService);

        if (_config.LogSensitiveMaterial)
            LogSigningKey(signingKey);
        else
            SigningKeyCreated();

        return signingKey;
    }
}