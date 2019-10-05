using System.IO;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts.Operations;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Fluid
{
    [PublicAPI]
    public class Transfer
    {
        private readonly IObjectOperations _objectOperations;

        public Transfer(IObjectOperations objectOperations)
        {
            _objectOperations = objectOperations;
        }

        public Upload UploadStream(string bucket, string resource, Stream stream, bool ownStream = false)
        {
            return new Upload(_objectOperations, bucket, resource, stream, ownStream);
        }

        public Upload UploadFile(string bucket, string resource, string fileName)
        {
            return UploadStream(bucket, resource, File.OpenRead(fileName), true);
        }

        public Upload UploadData(string bucket, string resource, byte[] data)
        {
            return UploadStream(bucket, resource, new MemoryStream(data), true);
        }

        public Upload UploadString(string bucket, string resource, string content, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            return UploadData(bucket, resource, encoding.GetBytes(content));
        }

        public Download Download(string bucket, string resource)
        {
            return new Download(_objectOperations, bucket, resource);
        }
    }
}