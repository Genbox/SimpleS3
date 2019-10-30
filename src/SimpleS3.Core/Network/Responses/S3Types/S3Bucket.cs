using System;

namespace Genbox.SimpleS3.Core.Network.Responses.S3Types
{
    public class S3Bucket
    {
        /// <summary>
        /// Name of the bucket
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The date the bucket was created
        /// </summary>
        public DateTimeOffset CreatedOn { get; internal set; }

        public override string ToString()
        {
            return Name;
        }
    }
}