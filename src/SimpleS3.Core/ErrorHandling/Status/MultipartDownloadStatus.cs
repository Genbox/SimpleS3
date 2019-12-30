namespace Genbox.SimpleS3.Core.ErrorHandling.Status
{
    public enum MultipartDownloadStatus
    {
        Unknown = 0,
        Ok,
        Error,
        Incomplete,
        NotFound,
        Aborted
    }
}