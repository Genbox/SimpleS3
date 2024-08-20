using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Transfer;

namespace Genbox.SimpleS3.Core.Internals.Fluent;

internal sealed class Transfer(IObjectOperations operations, IMultipartTransfer transfer) : ITransfer
{
    public IUpload CreateUpload(string bucket, string objectKey) => new Upload(operations, transfer, bucket, objectKey);

    public IDownload CreateDownload(string bucket, string objectKey) => new Download(operations, transfer, bucket, objectKey);
}