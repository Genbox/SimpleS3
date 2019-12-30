namespace Genbox.SimpleS3.Core.ErrorHandling.Status
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