using System;
using System.IO;

namespace Genbox.SimpleS3.Core.Tests.Code.Other
{
    public class NonSeekableStream : Stream
    {
        private readonly MemoryStream _backingStream;

        public NonSeekableStream(byte[] data)
        {
            _backingStream = new MemoryStream(data);
        }

        public override void Flush()
        {
            _backingStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _backingStream.Read(buffer, offset, count);
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

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => _backingStream.Position;
            set => _backingStream.Position = value;
        }
    }
}