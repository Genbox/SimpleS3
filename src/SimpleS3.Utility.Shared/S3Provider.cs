using System;

namespace Genbox.SimpleS3.Utility.Shared
{
    [Flags]
    public enum S3Provider
    {
        Unknown = 0,
        AmazonS3 = 1,
        BackBlazeB2 = 2,
        GoogleCloudStorage = 4,

        All = int.MaxValue
    }
}