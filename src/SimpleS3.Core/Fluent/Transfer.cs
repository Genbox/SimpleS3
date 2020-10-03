using Genbox.SimpleS3.Core.Abstracts.Operations;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Fluent
{
    [PublicAPI]
    public class Transfer
    {
        private readonly IMultipartOperations _multipartOperations;
        private readonly IObjectOperations _objectOperations;

        public Transfer(IObjectOperations objectOperations, IMultipartOperations multipartOperations)
        {
            _objectOperations = objectOperations;
            _multipartOperations = multipartOperations;
        }

        public Upload CreateUpload(string bucket, string objectKey)
        {
            return new Upload(_objectOperations, _multipartOperations, bucket, objectKey);
        }

        public Download CreateDownload(string bucket, string objectKey)
        {
            return new Download(_objectOperations, bucket, objectKey);
        }
    }
}