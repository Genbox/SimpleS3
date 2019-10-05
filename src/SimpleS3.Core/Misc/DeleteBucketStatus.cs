namespace Genbox.SimpleS3.Core.Misc
{
    public enum DeleteBucketStatus
    {
        Unknown = 0,
        Ok,
        FailedToDeleteObject,
        BucketNotEmpty
    }
}