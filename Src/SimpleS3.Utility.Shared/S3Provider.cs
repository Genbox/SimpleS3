namespace Genbox.SimpleS3.Utility.Shared;

[Flags]
public enum S3Provider
{
    None = 0,
    AmazonS3 = 1,
    BackBlazeB2 = 2,
    GoogleCloudStorage = 4,
    Wasabi = 8,

    All = AmazonS3 | BackBlazeB2 | GoogleCloudStorage | Wasabi
}