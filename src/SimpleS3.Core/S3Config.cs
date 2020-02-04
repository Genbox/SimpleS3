using System;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Authentication;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core
{
    [PublicAPI]
    public class S3Config : IConfig
    {
        public S3Config()
        {
        }

        public S3Config(IAccessKey credentials, AwsRegion region)
        {
            Credentials = credentials;
            Region = region;
        }

        public IAccessKey Credentials { get; set; }
        public AwsRegion Region { get; set; }
        public SignatureMode PayloadSignatureMode { get; set; } = SignatureMode.StreamingSignature;
        public int StreamingChunkSize { get; set; } = 2 * 1024 * 1024; // 2 Mb
        public NamingMode NamingMode { get; set; } = NamingMode.PathStyle;
        public Uri Endpoint { get; set; }
        public bool EnableBucketNameValidation { get; set; } = true;
        public KeyValidationMode ObjectKeyValidationMode { get; set; } = KeyValidationMode.AsciiMode;
        public bool AutoUrlDecodeResponses { get; set; } = true;
        public bool AlwaysCalculateContentMd5 { get; set; }
    }
}