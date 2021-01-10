using Genbox.SimpleS3.Core.Fluent;

namespace Genbox.SimpleS3.Core.Abstracts
{
    public interface ITransfer
    {
        Upload CreateUpload(string bucket, string objectKey);
        Download CreateDownload(string bucket, string objectKey);
    }
}