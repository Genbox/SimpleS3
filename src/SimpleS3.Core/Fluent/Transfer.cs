using System.IO;
using System.Text;
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

        public Upload UploadStream(string bucket, string objectKey, Stream stream, bool ownStream = false)
        {
            return new Upload(_objectOperations, _multipartOperations, bucket, objectKey, stream, ownStream);
        }

        public Upload UploadFile(string bucket, string objectKey, string fileName)
        {
            return UploadStream(bucket, objectKey, File.OpenRead(fileName), true);
        }

        public Upload UploadData(string bucket, string objectKey, byte[] data)
        {
            return UploadStream(bucket, objectKey, new MemoryStream(data), true);
        }

        public Upload UploadString(string bucket, string objectKey, string content, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            return UploadData(bucket, objectKey, encoding.GetBytes(content));
        }

        public Download Download(string bucket, string objectKey)
        {
            return new Download(_objectOperations, bucket, objectKey);
        }
    }
}