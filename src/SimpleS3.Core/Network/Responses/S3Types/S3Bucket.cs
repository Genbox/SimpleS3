using System;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types
{
    public class S3Bucket
    {
        public S3Bucket(string name, DateTimeOffset createdOn)
        {
            Name = name;
            CreatedOn = createdOn;
        }

        /// <summary>Name of the bucket</summary>
        public string Name { get; }

        /// <summary>The date the bucket was created</summary>
        public DateTimeOffset CreatedOn { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}