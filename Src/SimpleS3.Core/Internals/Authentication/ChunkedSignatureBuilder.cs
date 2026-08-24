using System.Globalization;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Helpers;
using Genbox.SimpleS3.Core.Common.Pools;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Internals.Extensions;
using Genbox.SimpleS3.Core.Internals.Misc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Internals.Authentication;

internal sealed partial class ChunkedSignatureBuilder(ISigningKeyBuilder keyBuilder, IScopeBuilder scopeBuilder, IOptions<SimpleS3Config> options, ILogger<ChunkedSignatureBuilder> logger) : IChunkedSignatureBuilder, ITrailerSignatureBuilder
{
    private readonly SimpleS3Config _config = options.Value;

    public byte[] CreateChunkSignature(IRequest request, byte[] previousSignature, byte[] content, int offset, int length)
    {
        Validator.RequireNotNull(request);

        CreatingChunkSignature(request.RequestId);

        string stringToSign = CreateStringToSign(request.Timestamp, scopeBuilder.CreateScope("s3", request.Timestamp), previousSignature, content, offset, length);
        byte[] signature = CreateSignature(request.Timestamp, stringToSign);

        if (_config.LogSensitiveMaterial)
            LogChunkSignature(signature);
        else
            ChunkSignatureCreated();

        return signature;
    }

    public byte[] CreateTrailerSignature(IRequest request, byte[] previousSignature)
    {
        Validator.RequireNotNull(request);

        CreatingTrailerSignature(request.RequestId);

        IReadOnlyDictionary<string, string> trailers = (request as IRequestTrailers)?.Trailers ?? EmptyTrailers;
        string canonicalTrailingHeaders = CreateCanonicalTrailingHeaders(trailers);
        string stringToSign = CreateTrailerStringToSign(request.Timestamp, scopeBuilder.CreateScope("s3", request.Timestamp), previousSignature, canonicalTrailingHeaders);
        byte[] signature = CreateSignature(request.Timestamp, stringToSign);

        if (_config.LogSensitiveMaterial)
            LogTrailerSignature(signature);
        else
            TrailerSignatureCreated();

        return signature;
    }

    internal string CreateStringToSign(DateTimeOffset dateTime, string scope, byte[] previousSignature, byte[] content, int offset, int length)
    {
        CreatingChunkedStringToSign();

        StringBuilder sb = StringBuilderPool.Shared.Rent(300);

        sb.Append(SigningConstants.ChunkedAlgorithmTag).Append(SigningConstants.Newline);
        sb.Append(dateTime.ToString(DateTimeFormats.Iso8601DateTime, DateTimeFormatInfo.InvariantInfo)).Append(SigningConstants.Newline);
        sb.Append(scope).Append(SigningConstants.Newline);
        sb.Append(previousSignature.HexEncode()).Append(SigningConstants.Newline);
        sb.Append("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855").Append(SigningConstants.Newline);
        sb.Append(CryptoHelper.Sha256Hash(content, offset, length).HexEncode());

        string sts = StringBuilderPool.Shared.ReturnString(sb, clearContents: true);
        if (_config.LogSensitiveMaterial)
            LogChunkedStringToSign(sts);

        return sts;
    }

    internal string CreateTrailerStringToSign(DateTimeOffset dateTime, string scope, byte[] previousSignature, string canonicalTrailingHeaders)
    {
        CreatingTrailerStringToSign();

        StringBuilder sb = StringBuilderPool.Shared.Rent(300);

        sb.Append(SigningConstants.TrailerAlgorithmTag).Append(SigningConstants.Newline);
        sb.Append(dateTime.ToString(DateTimeFormats.Iso8601DateTime, DateTimeFormatInfo.InvariantInfo)).Append(SigningConstants.Newline);
        sb.Append(scope).Append(SigningConstants.Newline);
        sb.Append(previousSignature.HexEncode()).Append(SigningConstants.Newline);
        sb.Append(CryptoHelper.Sha256Hash(Encoding.UTF8.GetBytes(canonicalTrailingHeaders)).HexEncode());

        string sts = StringBuilderPool.Shared.ReturnString(sb, clearContents: true);
        if (_config.LogSensitiveMaterial)
            LogTrailerStringToSign(sts);

        return sts;
    }

    internal static string CreateCanonicalTrailingHeaders(IReadOnlyDictionary<string, string> trailers)
    {
        StringBuilder sb = StringBuilderPool.Shared.Rent(200);

        foreach (KeyValuePair<string, string> item in trailers.OrderBy(x => x.Key.ToLowerInvariant(), StringComparer.Ordinal))
        {
            sb.Append(item.Key.ToLowerInvariant())
              .Append(SigningConstants.Colon)
              .Append(CanonicalizeHeaderValue(item.Value))
              .Append(SigningConstants.Newline);
        }

        return StringBuilderPool.Shared.ReturnString(sb, clearContents: true);
    }

    private static string CanonicalizeHeaderValue(string value) => string.Join(" ", value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));

    private static IReadOnlyDictionary<string, string> EmptyTrailers { get; } = new Dictionary<string, string>(0);

    /// <summary>See https://docs.aws.amazon.com/AmazonS3/latest/API/sig-v4-header-based-auth.html Consists of: Hmac-Sha256(SigningKey, StringToSign)</summary>
    internal byte[] CreateSignature(DateTimeOffset date, string stringToSign)
    {
        byte[]? stringToSignBytes = null;
        byte[]? signingKey = null;

        try
        {
            stringToSignBytes = Encoding.UTF8.GetBytes(stringToSign);
            signingKey = keyBuilder.CreateSigningKey(date);
            return CryptoHelper.HmacSign(stringToSignBytes, signingKey);
        }
        finally
        {
            if (stringToSignBytes != null)
                Array.Clear(stringToSignBytes, 0, stringToSignBytes.Length);

            if (signingKey != null)
                Array.Clear(signingKey, 0, signingKey.Length);
        }
    }
}