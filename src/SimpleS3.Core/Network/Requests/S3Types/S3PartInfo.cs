﻿using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types
{
    [PublicAPI]
    public class S3PartInfo
    {
        public S3PartInfo(string? eTag, int partNumber)
        {
            ETag = eTag;
            PartNumber = partNumber;
        }

        public string? ETag { get; }
        public int PartNumber { get; }
    }
}