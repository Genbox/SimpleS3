using System;
using Genbox.SimpleS3.Abstracts;
using Genbox.SimpleS3.Abstracts.Authentication;
using Genbox.SimpleS3.Abstracts.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core
{
    [PublicAPI]
    public class S3Config : IS3Config
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

        public bool EnablePayloadSigning { get; set; } = true;

        public bool EnableStreaming { get; set; } = true;

        public int StreamingChunkSize { get; set; } = 8 * 1024 * 1024; //8 Mb

        public NamingType NamingType { get; set; } = NamingType.PathStyle;

        public Uri Endpoint { get; set; }

        public bool EnableBucketNameValidation { get; set; } = true;

        public KeyValidationMode ObjectKeyValidationMode { get; set; } = KeyValidationMode.AsciiMode;
        
        public bool AutoUrlDecodeResponses { get; set; } = true;
    }
}