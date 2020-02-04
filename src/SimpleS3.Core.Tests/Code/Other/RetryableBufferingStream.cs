using System;
using System.IO;
using Genbox.SimpleS3.Core.Common;

namespace Genbox.SimpleS3.Core.Tests.Code.Other
{
    /// <summary>
    /// Stream that will buffer / record data as it's read, and be able to seek in it afterwards
    /// Used for retrying forward-only streams
    /// </summary>
    public class RetryableBufferingStream : Stream
    {
        private readonly Stream _underlyingStream;
        private readonly MemoryStream _bufferStream;

        public RetryableBufferingStream(Stream underlyingStream)
        {
            // Note: Maybe validation is superfluous?
            Validator.RequireThat(!underlyingStream.CanSeek, nameof(underlyingStream), $"The {nameof(RetryableBufferingStream)} should not be used on seekable streams");

            _underlyingStream = underlyingStream;
            _bufferStream = new MemoryStream();

            ReadSource();
        }

        private void ReadSource()
        {
            // TODO: Async? .. Perhaps do on Read(), Seek(), Length and Position?
            _underlyingStream.CopyTo(_bufferStream);
            _bufferStream.Seek(0, SeekOrigin.Begin);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _bufferStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _bufferStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => _bufferStream.Length;
        public override long Position
        {
            get => _bufferStream.Position;
            set => _bufferStream.Position = value;
        }
    }
}