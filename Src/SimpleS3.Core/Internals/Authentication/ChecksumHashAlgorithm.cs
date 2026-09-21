using System.Diagnostics.CodeAnalysis;
using System.IO.Hashing;
using System.Security.Cryptography;
using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Internals.Authentication;

[SuppressMessage("Security", "S2257", Justification = "CRC32 and CRC64 are non-cryptographic S3 checksums; HashAlgorithm adapts them to the shared streaming checksum API.")]
[SuppressMessage("Security", "S4790", Justification = "S3 requires this legacy hash for its wire-level checksum; it is not used for password storage or a new signature scheme.")]
[SuppressMessage("Security", "CA5350", Justification = "S3 requires this legacy hash for its wire-level checksum; it is not used for password storage or a new signature scheme.")]
[SuppressMessage("Security", "CA5351", Justification = "MD5 is an S3 wire checksum selected by the caller, not a security primitive.")]
internal static class ChecksumHashAlgorithm
{
    public static HashAlgorithm Create(ChecksumAlgorithm algorithm) => algorithm switch
    {
        ChecksumAlgorithm.Crc32 => new Crc32Algorithm(0xedb88320),
        ChecksumAlgorithm.Crc32C => new Crc32Algorithm(0x82f63b78),
        ChecksumAlgorithm.Crc64Nvme => new Crc64NvmeAlgorithm(),
        ChecksumAlgorithm.Sha1 => SHA1.Create(),
        ChecksumAlgorithm.Sha256 => SHA256.Create(),
        ChecksumAlgorithm.Sha512 => SHA512.Create(),
        ChecksumAlgorithm.Md5 => MD5.Create(),
        ChecksumAlgorithm.XxHash64 => new NonCryptographicHashAdapter(new XxHash64()),
        ChecksumAlgorithm.XxHash3 => new NonCryptographicHashAdapter(new XxHash3()),
        ChecksumAlgorithm.XxHash128 => new NonCryptographicHashAdapter(new XxHash128()),
        _ => throw new NotSupportedException($"Automatic streaming checksum calculation does not support {algorithm}. Provide a precalculated checksum value instead.")
    };

    public static int GetBase64Length(ChecksumAlgorithm algorithm) => algorithm switch
    {
        ChecksumAlgorithm.Crc32 or ChecksumAlgorithm.Crc32C => 8,
        ChecksumAlgorithm.Crc64Nvme => 12,
        ChecksumAlgorithm.Sha1 => 28,
        ChecksumAlgorithm.Sha256 => 44,
        ChecksumAlgorithm.Sha512 => 88,
        ChecksumAlgorithm.Md5 or ChecksumAlgorithm.XxHash128 => 24,
        ChecksumAlgorithm.XxHash64 or ChecksumAlgorithm.XxHash3 => 12,
        _ => throw new NotSupportedException($"Automatic streaming checksum calculation does not support {algorithm}. Provide a precalculated checksum value instead.")
    };

    private sealed class NonCryptographicHashAdapter : HashAlgorithm
    {
        private readonly NonCryptographicHashAlgorithm _algorithm;

        public NonCryptographicHashAdapter(NonCryptographicHashAlgorithm algorithm)
        {
            _algorithm = algorithm;
            HashSizeValue = algorithm.HashLengthInBytes * 8;
        }

        public override void Initialize() => _algorithm.Reset();
        protected override void HashCore(byte[] array, int ibStart, int cbSize) => _algorithm.Append(new ReadOnlySpan<byte>(array, ibStart, cbSize));
        protected override byte[] HashFinal() => _algorithm.GetCurrentHash();
    }

    private sealed class Crc32Algorithm : HashAlgorithm
    {
        private static readonly uint[] _crc32Table = CreateTable(0xedb88320);
        private static readonly uint[] _crc32CTable = CreateTable(0x82f63b78);
        private readonly uint[] _table;
        private uint _value = uint.MaxValue;

        public Crc32Algorithm(uint polynomial)
        {
            _table = polynomial == 0xedb88320 ? _crc32Table : _crc32CTable;
            HashSizeValue = 32;
        }

        public override void Initialize() => _value = uint.MaxValue;

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            for (int i = ibStart; i < ibStart + cbSize; i++)
                _value = (_value >> 8) ^ _table[(int)((_value ^ array[i]) & 0xff)];
        }

        protected override byte[] HashFinal()
        {
            uint value = _value ^ uint.MaxValue;

            unchecked
            {
                return [(byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value];
            }
        }

        private static uint[] CreateTable(uint polynomial)
        {
            uint[] table = new uint[256];

            for (uint i = 0; i < table.Length; i++)
            {
                uint value = i;

                for (int bit = 0; bit < 8; bit++)
                    value = (value >> 1) ^ ((value & 1) == 0 ? 0 : polynomial);

                table[i] = value;
            }

            return table;
        }
    }

    private sealed class Crc64NvmeAlgorithm : HashAlgorithm
    {
        private const ulong _polynomial = 0x9a6c9329ac4bc9b5;
        private static readonly ulong[] _table = CreateTable();
        private ulong _value = ulong.MaxValue;

        public Crc64NvmeAlgorithm()
        {
            HashSizeValue = 64;
        }

        public override void Initialize() => _value = ulong.MaxValue;

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            for (int i = ibStart; i < ibStart + cbSize; i++)
                _value = (_value >> 8) ^ _table[(int)((_value ^ array[i]) & 0xff)];
        }

        protected override byte[] HashFinal()
        {
            ulong value = _value ^ ulong.MaxValue;

            unchecked
            {
                return
                [
                    (byte)(value >> 56),
                    (byte)(value >> 48),
                    (byte)(value >> 40),
                    (byte)(value >> 32),
                    (byte)(value >> 24),
                    (byte)(value >> 16),
                    (byte)(value >> 8),
                    (byte)value
                ];
            }
        }

        private static ulong[] CreateTable()
        {
            ulong[] table = new ulong[256];

            for (ulong i = 0; i < (ulong)table.Length; i++)
            {
                ulong value = i;

                for (int bit = 0; bit < 8; bit++)
                    value = (value >> 1) ^ ((value & 1) == 0 ? 0 : _polynomial);

                table[i] = value;
            }

            return table;
        }
    }
}