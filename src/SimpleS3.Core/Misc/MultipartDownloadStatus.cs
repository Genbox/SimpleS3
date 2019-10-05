namespace Genbox.SimpleS3.Core.Misc
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