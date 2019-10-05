namespace Genbox.SimpleS3.Core.Misc
{
    public enum MultipartUploadStatus
    {
        Unknown = 0,
        Ok,
        Error,
        InitFailed,
        Incomplete
    }
}