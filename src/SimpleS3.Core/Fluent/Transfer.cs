using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Fluent
{
    [PublicAPI]
    public class Transfer : ITransfer
    {
        private readonly IMultipartTransfer _multipartTransfer;
        private readonly IObjectOperations _objectOperations;

        public Transfer(IObjectOperations objectOperations, IMultipartTransfer multipartTransfer)
        {
            _objectOperations = objectOperations;
            _multipartTransfer = multipartTransfer;
        }

        public Upload CreateUpload(string bucket, string objectKey)
        {
            return new Upload(_objectOperations, _multipartTransfer, bucket, objectKey);
        }

        public Download CreateDownload(string bucket, string objectKey)
        {
            return new Download(_objectOperations, _multipartTransfer, bucket, objectKey);
        }
    }
}