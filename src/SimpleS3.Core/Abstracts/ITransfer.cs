namespace Genbox.SimpleS3.Core.Abstracts
{
    public interface ITransfer
    {
        IUpload CreateUpload(string bucket, string objectKey);
        IDownload CreateDownload(string bucket, string objectKey);
    }
}