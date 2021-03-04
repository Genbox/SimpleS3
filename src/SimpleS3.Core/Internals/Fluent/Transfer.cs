using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using Genbox.SimpleS3.Core.Abstracts.Transfer;

namespace Genbox.SimpleS3.Core.Internals.Fluent
{
    internal class Transfer : ITransfer
    {
        private readonly IMultipartTransfer _multipartTransfer;
        private readonly IObjectOperations _objectOperations;

        public Transfer(IObjectOperations objectOperations, IMultipartTransfer multipartTransfer)
        {
            _objectOperations = objectOperations;
            _multipartTransfer = multipartTransfer;
        }

        public IUpload CreateUpload(string bucket, string objectKey)
        {
            return new Upload(_objectOperations, _multipartTransfer, bucket, objectKey);
        }

        public IDownload CreateDownload(string bucket, string objectKey)
        {
            return new Download(_objectOperations, _multipartTransfer, bucket, objectKey);
        }
    }
}