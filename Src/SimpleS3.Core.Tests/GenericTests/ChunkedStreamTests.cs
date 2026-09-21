using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using FluentValidation;
using FluentValidation.Results;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Misc;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Internals.Authentication;
using Genbox.SimpleS3.Core.Internals.Marshallers.Requests;
using Genbox.SimpleS3.Core.Internals.Network;
using Genbox.SimpleS3.Core.Internals.Validation.Validators.Configs;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class ChunkedStreamTests
{
    [Theory]
    [InlineData(8095, false)]
    [InlineData(8096, false)]
    [InlineData(8191, false)]
    [InlineData(8192, true)]
    public void ConfigRejectsChunkSizesBelowEightKiB(int size, bool expectedValid)
    {
        SimpleS3Config config = new SimpleS3Config(null, "{Scheme}://{Bucket:.}s3.example.test", "region") { StreamingChunkSize = size };
        ConfigValidator validator = new ConfigValidator(new InlineValidator<IAccessKey?>());
        ValidationResult result = validator.Validate(config);
        Assert.Equal(expectedValid, result.IsValid);

        if (!expectedValid)
        {
            ValidationFailure error = Assert.Single(result.Errors);
            Assert.Equal(nameof(SimpleS3Config.StreamingChunkSize), error.PropertyName);
            Assert.Contains("8192", error.ErrorMessage, StringComparison.Ordinal);
        }

        Assert.Equal(size, config.StreamingChunkSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(8192)]
    [InlineData(8193)]
    public void EmittedByteCountMatchesLength(int payloadLength)
    {
        byte[] payload = Enumerable.Repeat((byte)'a', payloadLength).ToArray();
        SimpleS3Config config = new SimpleS3Config("Test", "https://s3.example.com") { StreamingChunkSize = 8192 };
        using MemoryStream source = new MemoryStream(payload);
        using ChunkedStream stream = new ChunkedStream(config, new StableChunkedSignatureBuilder(), new PutObjectRequest("bucket", "object", null), new byte[32], source);
        using MemoryStream output = new MemoryStream();

        stream.CopyTo(output);

        Assert.Equal(stream.Length, output.Length);
    }

    [Fact]
    public void ChunkedWrapperRejectsStreamsWithoutLength()
    {
        SimpleS3Config config = new SimpleS3Config("Test", "https://s3.example.com") { StreamingChunkSize = 8192 };
        ChunkedContentRequestStreamWrapper wrapper = new ChunkedContentRequestStreamWrapper(Options.Create(config), new StableChunkedSignatureBuilder(), new StableSignatureBuilder());

        using UnknownLengthStream source = new UnknownLengthStream();
        NotSupportedException ex = Assert.Throws<NotSupportedException>(() =>
        {
            using Stream wrapped = wrapper.Wrap(source, new PutObjectRequest("bucket", "object", null));
        });
        Assert.Contains("exposes Length", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EmittedTrailerByteCountMatchesLength()
    {
        byte[] payload = "abc"u8.ToArray();
        SimpleS3Config config = new SimpleS3Config("Test", "https://s3.example.com") { StreamingChunkSize = 8192 };
        PutObjectRequest request = new PutObjectRequest("bucket", "object", null);
        request.SetTrailer(AmzHeaders.XAmzChecksumCrc32C, "sOO8/Q==");

        using MemoryStream source = new MemoryStream(payload);
        using ChunkedStream stream = new ChunkedStream(config, new StableChunkedSignatureBuilder(), request, new byte[32], source);
        using MemoryStream output = new MemoryStream();

        stream.CopyTo(output);

        string actual = Encoding.UTF8.GetString(output.ToArray());
        string chunkSignature = new string('0', 64);
        string trailerSignature = string.Concat(Enumerable.Repeat("01", 32));
        string expected = "3;chunk-signature=" + chunkSignature + "\r\n" +
                          "abc\r\n" +
                          "0;chunk-signature=" + chunkSignature + "\r\n" +
                          "x-amz-checksum-crc32c:sOO8/Q==\r\n" +
                          "x-amz-trailer-signature:" + trailerSignature + "\r\n" +
                          "\r\n";

        Assert.Equal(expected, actual);
        Assert.Equal(stream.Length, output.Length);
    }

    [Fact]
    public void ChunkedWrapperSetsTrailerHeaders()
    {
        byte[] payload = "abc"u8.ToArray();
        SimpleS3Config config = new SimpleS3Config("Test", "https://s3.example.com") { StreamingChunkSize = 8192 };
        ChunkedContentRequestStreamWrapper wrapper = new ChunkedContentRequestStreamWrapper(Options.Create(config), new StableChunkedSignatureBuilder(), new StableSignatureBuilder());
        using MemoryStream source = new MemoryStream(payload);
        PutObjectRequest request = new PutObjectRequest("bucket", "object", source);
        request.SetTrailer(AmzHeaders.XAmzChecksumCrc32C, "sOO8/Q==");

        using Stream wrapped = wrapper.Wrap(request.Content!, request);

        Assert.Equal("aws-chunked", request.Headers[HttpHeaders.ContentEncoding]);
        Assert.Equal("3", request.Headers[AmzHeaders.XAmzDecodedContentLength]);
        Assert.Equal(ChunkedStream.ComputeChunkedContentLength(payload.Length, config.StreamingChunkSize, ChunkedStream.SignatureLength, request.Trailers).ToString(CultureInfo.InvariantCulture), request.Headers[HttpHeaders.ContentLength]);
        Assert.Equal("STREAMING-AWS4-HMAC-SHA256-PAYLOAD-TRAILER", request.Headers[AmzHeaders.XAmzContentSha256]);
        Assert.Equal(AmzHeaders.XAmzChecksumCrc32C, request.Headers[AmzHeaders.XAmzTrailer]);
    }

    [Fact]
    public void ContentEncodingRequiresExactAwsChunkedToken()
    {
        byte[] payload = "abc"u8.ToArray();
        SimpleS3Config config = new SimpleS3Config("Test", "https://s3.example.com") { StreamingChunkSize = 8192 };
        ChunkedContentRequestStreamWrapper wrapper = new ChunkedContentRequestStreamWrapper(Options.Create(config), new StableChunkedSignatureBuilder(), new StableSignatureBuilder());
        using MemoryStream source = new MemoryStream(payload);
        PutObjectRequest request = new PutObjectRequest("bucket", "object", source);
        request.SetHeader(HttpHeaders.ContentEncoding, "x-aws-chunked");

        using Stream wrapped = wrapper.Wrap(request.Content!, request);

        Assert.Equal("x-aws-chunked, aws-chunked", request.Headers[HttpHeaders.ContentEncoding]);
    }

    [Fact]
    public void SelectedChecksumIsCalculatedAsTrailer()
    {
        byte[] payload = "abc"u8.ToArray();
        SimpleS3Config config = new SimpleS3Config("Test", "https://s3.example.com") { StreamingChunkSize = 8192 };
        ChunkedContentRequestStreamWrapper wrapper = new ChunkedContentRequestStreamWrapper(Options.Create(config), new StableChunkedSignatureBuilder(), new StableSignatureBuilder());
        using MemoryStream source = new MemoryStream(payload);
        PutObjectRequest request = new PutObjectRequest("bucket", "object", source) { ChecksumAlgorithm = ChecksumAlgorithm.Sha256 };

        using Stream wrapped = wrapper.Wrap(request.Content!, request);
        long expectedLength = ChunkedStream.ComputeChunkedContentLength(payload.Length, config.StreamingChunkSize, ChunkedStream.SignatureLength, request.Trailers, ChecksumAlgorithm.Sha256);
        using MemoryStream output = new MemoryStream();
        wrapped.CopyTo(output);

        const string expectedChecksum = "ungWv48Bz+pBQUDeXa4iI7ADYaOWF3qctBD/YfIAFa0=";
        Assert.Equal("SHA256", request.Headers[AmzHeaders.XAmzSdkCheckumAlgorithm]);
        Assert.Equal(AmzHeaders.XAmzChecksumSha256, request.Headers[AmzHeaders.XAmzTrailer]);
        Assert.Equal(expectedLength.ToString(CultureInfo.InvariantCulture), request.Headers[HttpHeaders.ContentLength]);
        Assert.Equal(expectedLength, output.Length);
        Assert.Equal(string.Empty, request.Trailers[AmzHeaders.XAmzChecksumSha256]);
        Assert.Contains(AmzHeaders.XAmzChecksumSha256 + ":" + expectedChecksum + "\r\n", Encoding.UTF8.GetString(output.ToArray()), StringComparison.Ordinal);
    }

    [Fact]
    public void PositionedStreamUsesRemainingLengthAndRewindsToInitialPosition()
    {
        byte[] content = "prefix-payload"u8.ToArray();
        using MemoryStream source = new MemoryStream(content) { Position = 7 };
        SimpleS3Config config = new SimpleS3Config("Test", "https://s3.example.com") { StreamingChunkSize = 8192 };
        ChunkedContentRequestStreamWrapper wrapper = new ChunkedContentRequestStreamWrapper(Options.Create(config), new StableChunkedSignatureBuilder(), new StableSignatureBuilder());
        PutObjectRequest request = new PutObjectRequest("bucket", "object", source);

        using Stream wrapped = wrapper.Wrap(source, request);
        using MemoryStream first = new MemoryStream();
        using MemoryStream second = new MemoryStream();
        wrapped.CopyTo(first);
        wrapped.Seek(0, SeekOrigin.Begin);
        wrapped.CopyTo(second);

        string encoded = Encoding.UTF8.GetString(first.ToArray());
        Assert.Equal("7", request.Headers[AmzHeaders.XAmzDecodedContentLength]);
        Assert.Contains("\r\npayload\r\n", encoded, StringComparison.Ordinal);
        Assert.DoesNotContain("prefix-payload", encoded, StringComparison.Ordinal);
        Assert.Equal(wrapped.Length, first.Length);
        Assert.Equal(first.ToArray(), second.ToArray());
    }

    [Fact]
    public async Task SelectedChecksumSupportsSmallAsyncReads()
    {
        byte[] payload = "abc"u8.ToArray();
        SimpleS3Config config = new SimpleS3Config("Test", "https://s3.example.com") { StreamingChunkSize = 8192 };
        ChunkedContentRequestStreamWrapper wrapper = new ChunkedContentRequestStreamWrapper(Options.Create(config), new StableChunkedSignatureBuilder(), new StableSignatureBuilder());
        await using MemoryStream source = new MemoryStream(payload);
        PutObjectRequest request = new PutObjectRequest("bucket", "object", source) { ChecksumAlgorithm = ChecksumAlgorithm.Crc32C };

        await using Stream wrapped = wrapper.Wrap(request.Content!, request);
        await using MemoryStream output = new MemoryStream();
        byte[] buffer = new byte[1];
        int read;

        while ((read = await wrapped.ReadAsync(buffer.AsMemory(), TestContext.Current.CancellationToken)) > 0)
            await output.WriteAsync(buffer.AsMemory(0, read), TestContext.Current.CancellationToken);

        Assert.Equal(string.Empty, request.Trailers[AmzHeaders.XAmzChecksumCrc32C]);
        Assert.Contains(AmzHeaders.XAmzChecksumCrc32C + ":Nks/tw==\r\n", Encoding.UTF8.GetString(output.ToArray()), StringComparison.Ordinal);
        Assert.Equal(wrapped.Length, output.Length);
    }

    [Fact]
    public void ZeroLengthReadDoesNotConsumeTrailers()
    {
        (Stream wrapped, Stream source, long trailerOffset) = CreateStreamWithTrailer();

        using (wrapped)
        using (source)
        using (MemoryStream output = new MemoryStream())
        {
            CopyBytes(wrapped, output, trailerOffset);

            Assert.Equal(0, wrapped.Read([], 0, 0));
            wrapped.CopyTo(output);

            Assert.Equal(wrapped.Length, output.Length);
            Assert.Contains("x-amz-trailer-signature:", Encoding.UTF8.GetString(output.ToArray()), StringComparison.Ordinal);
        }
    }

    [Fact]
    public async Task ZeroLengthAsyncReadDoesNotConsumeTrailers()
    {
        (Stream wrapped, Stream source, long trailerOffset) = CreateStreamWithTrailer();

        await using (wrapped)
        await using (source)
        await using (MemoryStream output = new MemoryStream())
        {
            CopyBytes(wrapped, output, trailerOffset);

            Assert.Equal(0, await wrapped.ReadAsync(Memory<byte>.Empty, TestContext.Current.CancellationToken));
            await wrapped.CopyToAsync(output, TestContext.Current.CancellationToken);

            Assert.Equal(wrapped.Length, output.Length);
            Assert.Contains("x-amz-trailer-signature:", Encoding.UTF8.GetString(output.ToArray()), StringComparison.Ordinal);
        }
    }

    [Fact]
    public void PrecalculatedChecksumRemainsAHeader()
    {
        byte[] payload = "abc"u8.ToArray();
        byte[] checksum = SHA256.HashData(payload);
        SimpleS3Config config = new SimpleS3Config("Test", "https://s3.example.com") { StreamingChunkSize = 8192 };
        ChunkedContentRequestStreamWrapper wrapper = new ChunkedContentRequestStreamWrapper(Options.Create(config), new StableChunkedSignatureBuilder(), new StableSignatureBuilder());
        using MemoryStream source = new MemoryStream(payload);
        PutObjectRequest request = new PutObjectRequest("bucket", "object", source)
        {
            ChecksumAlgorithm = ChecksumAlgorithm.Sha256,
            Checksum = checksum
        };
        GenericRequestMapper.Map(request);

        using Stream wrapped = wrapper.Wrap(request.Content!, request);

        Assert.Equal(Convert.ToBase64String(checksum), request.Headers[AmzHeaders.XAmzChecksumSha256]);
        Assert.Empty(request.Trailers);
        Assert.Equal("STREAMING-AWS4-HMAC-SHA256-PAYLOAD", request.Headers[AmzHeaders.XAmzContentSha256]);
        Assert.DoesNotContain(AmzHeaders.XAmzSdkCheckumAlgorithm, request.Headers);
    }

    [Fact]
    public void RewindRecreatesChecksumAndTrailers()
    {
        byte[] payload = "abc"u8.ToArray();
        SimpleS3Config config = new SimpleS3Config("Test", "https://s3.example.com") { StreamingChunkSize = 8192 };
        ChunkedContentRequestStreamWrapper wrapper = new ChunkedContentRequestStreamWrapper(Options.Create(config), new StableChunkedSignatureBuilder(), new StableSignatureBuilder());
        using MemoryStream source = new MemoryStream(payload);
        PutObjectRequest request = new PutObjectRequest("bucket", "object", source) { ChecksumAlgorithm = ChecksumAlgorithm.Sha256 };
        using Stream wrapped = wrapper.Wrap(request.Content!, request);
        using MemoryStream first = new MemoryStream();
        using MemoryStream second = new MemoryStream();

        wrapped.CopyTo(first);
        wrapped.Seek(0, SeekOrigin.Begin);
        wrapped.CopyTo(second);

        Assert.Equal(first.ToArray(), second.ToArray());
        Assert.Equal(string.Empty, request.Trailers[AmzHeaders.XAmzChecksumSha256]);
    }

    [Fact]
    public void ReusedRequestRecalculatesAutomaticChecksum()
    {
        byte[] firstPayload = "abc"u8.ToArray();
        byte[] secondPayload = "def"u8.ToArray();
        using MemoryStream source = new MemoryStream(firstPayload.ToArray());
        SimpleS3Config config = new SimpleS3Config("Test", "https://s3.example.com") { StreamingChunkSize = 8192 };
        ChunkedContentRequestStreamWrapper wrapper = new ChunkedContentRequestStreamWrapper(Options.Create(config), new StableChunkedSignatureBuilder(), new StableSignatureBuilder());
        PutObjectRequest request = new PutObjectRequest("bucket", "object", source) { ChecksumAlgorithm = ChecksumAlgorithm.Sha256 };
        using MemoryStream firstOutput = new MemoryStream();
        using MemoryStream secondOutput = new MemoryStream();

        using (Stream wrapped = wrapper.Wrap(source, request))
            wrapped.CopyTo(firstOutput);

        request.ClearTransportState();
        source.Position = 0;
        source.Write(secondPayload, 0, secondPayload.Length);
        source.Position = 0;

        using (Stream wrapped = wrapper.Wrap(source, request))
            wrapped.CopyTo(secondOutput);

        string firstChecksum = Convert.ToBase64String(SHA256.HashData(firstPayload));
        string secondChecksum = Convert.ToBase64String(SHA256.HashData(secondPayload));
        Assert.Contains(AmzHeaders.XAmzChecksumSha256 + ":" + firstChecksum + "\r\n", Encoding.UTF8.GetString(firstOutput.ToArray()), StringComparison.Ordinal);
        Assert.Contains(AmzHeaders.XAmzChecksumSha256 + ":" + secondChecksum + "\r\n", Encoding.UTF8.GetString(secondOutput.ToArray()), StringComparison.Ordinal);
        Assert.NotEqual(firstChecksum, secondChecksum, StringComparer.Ordinal);
        Assert.Equal(string.Empty, request.Trailers[AmzHeaders.XAmzChecksumSha256]);
    }

    [Theory]
    [InlineData(ChecksumAlgorithm.Crc32, "y/Q5Jg==")]
    [InlineData(ChecksumAlgorithm.Crc32C, "4waSgw==")]
    [InlineData(ChecksumAlgorithm.Crc64Nvme, "rosUhgp5mIg=")]
    public void ChecksumAlgorithmsMatchKnownVectors(ChecksumAlgorithm algorithm, string expected)
    {
        using HashAlgorithm hash = ChecksumHashAlgorithm.Create(algorithm);

        Assert.Equal(expected, Convert.ToBase64String(hash.ComputeHash("123456789"u8.ToArray())));
    }

    [Fact]
    public void UnknownAutomaticChecksumIsRejected()
    {
        byte[] payload = "abc"u8.ToArray();
        SimpleS3Config config = new SimpleS3Config("Test", "https://s3.example.com") { StreamingChunkSize = 8192 };
        ChunkedContentRequestStreamWrapper wrapper = new ChunkedContentRequestStreamWrapper(Options.Create(config), new StableChunkedSignatureBuilder(), new StableSignatureBuilder());
        using MemoryStream input = new MemoryStream(payload);
        PutObjectRequest request = new PutObjectRequest("bucket", "object", input) { ChecksumAlgorithm = (ChecksumAlgorithm)int.MaxValue };

        NotSupportedException exception = Assert.Throws<NotSupportedException>(() =>
        {
            using Stream result = wrapper.Wrap(request.Content!, request);
        });

        Assert.Contains("precalculated", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ResetClearsTrailers()
    {
        PutObjectRequest request = new PutObjectRequest("bucket", "object", null);
        request.SetTrailer(AmzHeaders.XAmzChecksumCrc32C, "sOO8/Q==");

        request.Reset();

        Assert.Empty(request.Trailers);
    }

    [Fact]
    public void ClearingTransportStatePreservesTrailers()
    {
        PutObjectRequest request = new PutObjectRequest("bucket", "object", null);
        request.SetHeader(AmzHeaders.XAmzDate, "date");
        request.SetTrailer(AmzHeaders.XAmzChecksumCrc32C, "sOO8/Q==");

        request.ClearTransportState();

        Assert.Empty(request.Headers);
        Assert.Equal("sOO8/Q==", request.Trailers[AmzHeaders.XAmzChecksumCrc32C]);
    }

    [Fact]
    public void TrailerNamesAreNormalizedAndInvalidFramingIsRejected()
    {
        PutObjectRequest request = new PutObjectRequest("bucket", "object", null);

        request.SetTrailer("X-Custom_Trailer", "value");

        Assert.Equal("value", request.Trailers["x-custom_trailer"]);
        Assert.Throws<ArgumentException>(() => request.SetTrailer("invalid:name", "value"));
        Assert.Throws<ArgumentException>(() => request.SetTrailer("x-custom", "value\r\ninjected:value"));
        Assert.Throws<ArgumentException>(() => request.SetTrailer("x-amz-trailer-signature", "value"));
    }

    private static (Stream Stream, Stream Source, long TrailerOffset) CreateStreamWithTrailer()
    {
        SimpleS3Config config = new SimpleS3Config("Test", "https://s3.example.com") { StreamingChunkSize = 8192 };
        PutObjectRequest request = new PutObjectRequest("bucket", "object", null);
        request.SetTrailer(AmzHeaders.XAmzChecksumCrc32C, "sOO8/Q==");
        MemoryStream source = new MemoryStream("abc"u8.ToArray());
        ChunkedStream stream = new ChunkedStream(config, new StableChunkedSignatureBuilder(), request, new byte[32], source);
        int trailerLength = Encoding.UTF8.GetByteCount(AmzHeaders.XAmzChecksumCrc32C + ":sOO8/Q==\r\n" +
                                                       "x-amz-trailer-signature:" + new string('0', 64) + "\r\n\r\n");
        return (stream, source, stream.Length - trailerLength);
    }

    private static void CopyBytes(Stream input, Stream output, long count)
    {
        byte[] buffer = new byte[128];

        while (count > 0)
        {
            int read = input.Read(buffer, 0, (int)Math.Min(buffer.Length, count));
            Assert.True(read > 0);
            output.Write(buffer, 0, read);
            count -= read;
        }
    }

    private sealed class StableChunkedSignatureBuilder : IChunkedSignatureBuilder, ITrailerSignatureBuilder
    {
        public byte[] CreateChunkSignature(IRequest request, byte[] previousSignature, byte[] content, int offset, int length) => new byte[32];

        public byte[] CreateTrailerSignature(IRequest request, byte[] previousSignature) => Enumerable.Repeat((byte)1, 32).ToArray();
    }

    private sealed class StableSignatureBuilder : ISignatureBuilder
    {
        public byte[] CreateSignature(IRequest request, string service, bool enablePayloadSignature = true) => new byte[32];
    }

    private sealed class UnknownLengthStream : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count) => 0;
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void Flush() {}
    }
}