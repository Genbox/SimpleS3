namespace Genbox.SimpleS3.Core.Abstracts.Transfer;

/// <summary>Provides the <c>ITransfer</c> API.</summary>
public interface ITransfer
{
    /// <summary>Provides the <c>CreateUpload</c> API.</summary>
    IUpload CreateUpload(string bucket, string objectKey);
    /// <summary>Provides the <c>CreateDownload</c> API.</summary>
    IDownload CreateDownload(string bucket, string objectKey);
}