using System.Text;

namespace Genbox.SimpleS3.Core.Common.Constants
{
    public static class Constants
    {
        public static readonly string DefaultUserAgent = "SimpleS3 Client " + typeof(Constants).Assembly.GetName().Version;
        public static readonly string EmptyMd5 = "d41d8cd98f00b204e9800998ecf8427e";
        public static readonly byte[] EmptyMd5Bytes = { 0xd4, 0x1d, 0x8c, 0xd9, 0x8f, 0x00, 0xb2, 0x04, 0xe9, 0x80, 0x09, 0x98, 0xec, 0xf8, 0x42, 0x7e };
        public static readonly string EmptySha256 = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
        public static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);
        public static readonly string AmzMetadata = "x-amz-meta-";
    }
}