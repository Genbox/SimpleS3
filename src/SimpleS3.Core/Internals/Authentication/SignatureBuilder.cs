using System.Globalization;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Authentication;
using Genbox.SimpleS3.Core.Common.Constants;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Pools;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Genbox.SimpleS3.Core.Internals.Misc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Authentication;

internal class SignatureBuilder : ISignatureBuilder
{
    //https://docs.aws.amazon.com/AmazonS3/latest/API/sig-v4-authenticating-requests.html

    //Header looks like this:
    // Authorization: AWS4-HMAC-SHA256 Credential=AKIAIOSFODNN7EXAMPLE/20130524/us-east-1/s3/aws4_request, SignedHeaders=host;range;x-amz-date, Signature=fe5f80f77d5fa3beca038a248ff027d0445342fe2855ddc963176630326f1024

    //AWS4-HMAC-SHA256
    // The algorithm that was used to calculate the signature.

    //Credential
    // Built from '<your-access-key-id>/<date>/<aws-region>/<aws-service>/aws4_request'
    // <date> is in YYYYMMDD format
    // <aws-region> is the endpoint to send to
    // <aws-service> should be s3

    //SignedHeaders
    // A semicolon-separated list of request headers that you used to compute Signature. The list includes header names only, and the header names must be in lowercase.

    //Signature
    // The 256-bit signature expressed as 64 lowercase hexadecimal characters.

    private readonly ISigningKeyBuilder _keyBuilder;
    private readonly ILogger<SignatureBuilder> _logger;
    private readonly SimpleS3Config _options;
    private readonly IScopeBuilder _scopeBuilder;

    public SignatureBuilder(ISigningKeyBuilder keyBuilder, IScopeBuilder scopeBuilder, IOptions<SimpleS3Config> options, ILogger<SignatureBuilder> logger)
    {
        _keyBuilder = keyBuilder;
        _scopeBuilder = scopeBuilder;
        _options = options.Value;
        _logger = logger;
    }

    public byte[] CreateSignature(IRequest request, bool enablePayloadSignature = true)
    {
        Validator.RequireNotNull(request);

        _logger.LogTrace("Creating signature for {RequestId}", request.RequestId);

        StringBuilder sb = StringBuilderPool.Shared.Rent(200);
        RequestHelper.AppendPath(sb, _options, request);
        string url = StringBuilderPool.Shared.ReturnString(sb);

        string payloadSignature = enablePayloadSignature ? request.Headers[AmzHeaders.XAmzContentSha256] : "UNSIGNED-PAYLOAD";
        string canonicalRequest = CreateCanonicalRequest(request.RequestId, url, request.Method, request.Headers, request.QueryParameters, payloadSignature);
        string stringToSign = CreateStringToSign(request.Timestamp, _scopeBuilder.CreateScope("s3", request.Timestamp), canonicalRequest);
        byte[] signature = CreateSignature(request.Timestamp, stringToSign);

        _logger.LogDebug("Signature: {Signature}", signature);
        return signature;
    }

    /// <summary>See https://docs.aws.amazon.com/AmazonS3/latest/API/sig-v4-header-based-auth.html Consists of:
    /// Hmac-Sha256(SigningKey, StringToSign)</summary>
    internal byte[] CreateSignature(DateTimeOffset date, string stringToSign) => CryptoHelper.HmacSign(Encoding.UTF8.GetBytes(stringToSign), _keyBuilder.CreateSigningKey(date));

    internal string CreateCanonicalRequest(Guid requestId, string url, HttpMethodType method, IReadOnlyDictionary<string, string> headers, IReadOnlyDictionary<string, string> query, string contentHash)
    {
        _logger.LogTrace("Creating canonical request for {RequestId}", requestId);

        //Consists of:
        // HTTP Verb + \n
        // Canonical URI + \n
        // Canonical Query String + \n
        // Canonical Headers + \n
        // Signed Headers + \n
        // Sha256 Content Hash

        StringBuilder sb = StringBuilderPool.Shared.Rent(300);
        sb.Append(method.ToString()).Append(SigningConstants.Newline);
        sb.Append(url).Append(SigningConstants.Newline);
        sb.Append(CanonicalizeQueryParameters(query)).Append(SigningConstants.Newline);

        //Headers needs to be ordered by key
        OrderedDictionary<string, string> orderedHeaders = new OrderedDictionary<string, string>(HeaderWhitelist.FilterHeaders(headers));

        sb.Append(CanonicalizeHeaders(orderedHeaders)).Append(SigningConstants.Newline);
        sb.Append(CanonicalizeHeaderNames(orderedHeaders)).Append(SigningConstants.Newline);
        sb.Append(contentHash);

        string canonicalRequest = StringBuilderPool.Shared.ReturnString(sb);

        _logger.LogDebug("CanonicalRequest: {CanonicalRequest}", canonicalRequest);
        return canonicalRequest;
    }

    internal string CreateStringToSign(DateTimeOffset dateTime, string scope, string canonicalRequest)
    {
        _logger.LogTrace("Creating StringToSign");

        //Consists of:
        // "AWS4-HMAC-SHA256" + \n
        // TimeStamp + \n
        // Scope + \n
        // Hex(SHA256(Canonical Request))

        StringBuilder sb = StringBuilderPool.Shared.Rent(150);
        sb.Append(SigningConstants.AlgorithmTag).Append(SigningConstants.Newline);
        sb.Append(dateTime.ToString(DateTimeFormats.Iso8601DateTime, DateTimeFormatInfo.InvariantInfo)).Append(SigningConstants.Newline); //See https://docs.aws.amazon.com/general/latest/gr/sigv4-date-handling.html
        sb.Append(scope).Append(SigningConstants.Newline);
        sb.Append(CryptoHelper.Sha256Hash(Encoding.UTF8.GetBytes(canonicalRequest)).HexEncode());

        string sts = StringBuilderPool.Shared.ReturnString(sb);

        _logger.LogDebug("StringToSign: {StringToSign}", sts);
        return sts;
    }

    private static string CanonicalizeQueryParameters(IReadOnlyDictionary<string, string> parameters)
    {
        if (parameters.Count == 0)
            return string.Empty;

        //Source: https://docs.aws.amazon.com/AmazonS3/latest/API/sig-v4-header-based-auth.html
        //1. You URI-encode name and values individually
        //2. You must also sort the parameters in the canonical query string alphabetically by key name.
        //3. The sorting occurs after encoding.
        //4. The query string in the following URI example is prefix=somePrefix&marker=someMarker&max-keys=20:
        //5. If the URI does not include a '?', there is no query string in the request, and you set the canonical query string to an empty string (""). You will still need to include the "\n".

        return UrlHelper.CreateQueryString(parameters.OrderBy(kvp => kvp.Key, StringComparer.Ordinal), outputEqualOnEmpty: true);
    }

    private static string CanonicalizeHeaders(IReadOnlyDictionary<string, string> headers)
    {
        if (headers.Count == 0)
            return string.Empty;

        //Source: https://docs.aws.amazon.com/AmazonS3/latest/API/sig-v4-header-based-auth.html
        //1. Individual header name and value pairs are separated by the newline character ("\n")
        //2. Header names must be in lowercase.
        //3. You must sort the header names alphabetically

        StringBuilder sb = StringBuilderPool.Shared.Rent(200);

        foreach (KeyValuePair<string, string> item in headers)
        {
            sb.Append(item.Key)
              .Append(SigningConstants.Colon)
              .Append(item.Value)
              .Append(SigningConstants.Newline);
        }

        return StringBuilderPool.Shared.ReturnString(sb);
    }

    private static string CanonicalizeHeaderNames(IReadOnlyDictionary<string, string> headers)
    {
        if (headers.Count == 0)
            return string.Empty;

        //Source: https://docs.aws.amazon.com/AmazonS3/latest/API/sig-v4-header-based-auth.html
        //1. Alphabetically sorted, semicolon-separated list of lowercase request header names

        StringBuilder sb = StringBuilderPool.Shared.Rent(50);

        foreach (KeyValuePair<string, string> item in headers)
        {
            if (sb.Length > 0)
                sb.Append(SigningConstants.SemiColon);

            sb.Append(item.Key);
        }

        return StringBuilderPool.Shared.ReturnString(sb);
    }
}