using System;
using System.Globalization;
using System.IO;
using System.Text;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Internal.Extensions;
using Genbox.SimpleS3.Utils;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Authentication
{
    public class ChunkedStream : Stream
    {
        private const string _newlineStr = "\r\n";
        private const string _chunkSignature = ";chunk-signature=";

        private static readonly byte[] Newline = Encoding.UTF8.GetBytes(_newlineStr);
        private readonly IChunkedSignatureBuilder _chunkedSigBuilder;
        private readonly int _chunkSize;

        private readonly byte[] _inputBuffer;
        private readonly Stream _originalStream;
        private readonly byte[] _outputBuffer;
        private readonly IRequest _request;
        private bool _inputStreamConsumed;

        private bool _outputBufferIsTerminatingChunk;

        private int _outputBufferLength = -1;
        private int _outputBufferPosition = -1;
        private byte[] _previousSignature;

        public ChunkedStream(IOptions<S3Config> options, IChunkedSignatureBuilder chunkedSigBuilder, IRequest request, byte[] seedSignature, Stream originalStream)
        {
            Validator.RequireNotNull(options, nameof(options));
            Validator.RequireNotNull(chunkedSigBuilder, nameof(chunkedSigBuilder));
            Validator.RequireNotNull(request, nameof(request));
            Validator.RequireNotNull(seedSignature, nameof(seedSignature));
            Validator.RequireNotNull(originalStream, nameof(originalStream));

            _originalStream = originalStream;
            _chunkedSigBuilder = chunkedSigBuilder;
            _request = request;
            _previousSignature = seedSignature;

            _chunkSize = options.Value.StreamingChunkSize;

            _inputBuffer = new byte[_chunkSize];
            _outputBuffer = new byte[CalculateChunkHeaderLength(_chunkSize, seedSignature.Length * 2)];
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => ComputeChunkedContentLength(_originalStream.Length);
        public override long Position { get; set; }

        public override void Flush()
        {
            //Do nothing
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_outputBufferPosition == -1)
            {
                if (_inputStreamConsumed && _outputBufferIsTerminatingChunk)
                    return 0;

                int bytesRead = FillInputBuffer();
                FillOutputBuffer(bytesRead);
                _outputBufferIsTerminatingChunk = _inputStreamConsumed && bytesRead == 0;
            }

            //Use the smaller of either data we have remaining, or the count being asked for
            count = Math.Min(_outputBufferLength - _outputBufferPosition, count);

            //Copy part of our output buffer into buffer
            Buffer.BlockCopy(_outputBuffer, _outputBufferPosition, buffer, offset, count);
            _outputBufferPosition += count;

            if (_outputBufferPosition >= _outputBufferLength)
                _outputBufferPosition = -1;

            return count;
        }

        private void FillOutputBuffer(int bytesRead)
        {
            //Create signature and header
            _previousSignature = _chunkedSigBuilder.CreateChunkSignature(_request, _previousSignature, _inputBuffer, bytesRead);
            byte[] chunkHeader = CreateChunkHeader(_previousSignature, bytesRead);

            _outputBufferLength = 0;
            _outputBufferPosition = 0;

            //Copy the chunk header
            Buffer.BlockCopy(chunkHeader, 0, _outputBuffer, _outputBufferLength, chunkHeader.Length);
            _outputBufferLength += chunkHeader.Length;

            //Copy the input buffer (if any)
            if (bytesRead > 0)
            {
                Buffer.BlockCopy(_inputBuffer, 0, _outputBuffer, _outputBufferLength, bytesRead);
                _outputBufferLength += bytesRead;
            }

            //Copy the final newline
            Buffer.BlockCopy(Newline, 0, _outputBuffer, _outputBufferLength, Newline.Length);
            _outputBufferLength += Newline.Length;
        }

        private int FillInputBuffer()
        {
            if (_inputStreamConsumed)
                return 0;

            int totalRead = 0;

            while (totalRead < _inputBuffer.Length && !_inputStreamConsumed)
            {
                int remaining = Math.Min(_chunkSize, _inputBuffer.Length - totalRead);
                int bytesRead = _originalStream.Read(_inputBuffer, totalRead, remaining);

                if (bytesRead == 0)
                    _inputStreamConsumed = true;
                else
                    totalRead += bytesRead;
            }

            return totalRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Computes the total size of the data payload, including the chunk metadata. Called externally so as to be able to set the correct
        /// Content-Length header value.
        /// </summary>
        private long ComputeChunkedContentLength(long originalLength)
        {
            if (originalLength < 0)
                throw new ArgumentOutOfRangeException(nameof(originalLength), "Expected 0 or greater value for originalLength.");

            if (originalLength == 0)
                return CalculateChunkHeaderLength(0, 64);

            long maxSizeChunks = originalLength / _chunkSize;
            long remainingBytes = originalLength % _chunkSize;
            return maxSizeChunks * CalculateChunkHeaderLength(_chunkSize, 64)
                   + (remainingBytes > 0 ? CalculateChunkHeaderLength(remainingBytes, 64) : 0)
                   + CalculateChunkHeaderLength(0, 64);
        }

        private static long CalculateChunkHeaderLength(long chunkSize, int signatureLength)
        {
            return chunkSize.ToString("X", NumberFormatInfo.InvariantInfo).Length
                   + _chunkSignature.Length
                   + signatureLength
                   + Newline.Length
                   + chunkSize
                   + Newline.Length;
        }

        private static byte[] CreateChunkHeader(byte[] chunkSignature, int contentLength)
        {
            StringBuilder chunkHeader = new StringBuilder();
            chunkHeader
                .Append(contentLength.ToString("X", CultureInfo.InvariantCulture))
                .Append(_chunkSignature)
                .Append(chunkSignature.HexEncode())
                .Append(_newlineStr);

            return Encoding.UTF8.GetBytes(chunkHeader.ToString());
        }
    }
}