using System;

namespace Genbox.SimpleS3.Core.Responses.S3Types
{
    public class S3Bucket
    {
        public string Name { get; set; }
        public DateTimeOffset CreationDate { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}