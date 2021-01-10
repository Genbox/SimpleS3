﻿using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types
{
    public class S3NonCurrentVersionTransition
    {
        public S3NonCurrentVersionTransition(int nonCurrentDays, StorageClass storageClass)
        {
            NonCurrentDays = nonCurrentDays;
            StorageClass = storageClass;
        }

        /// <summary>Specifies the number of days an object is noncurrent before Amazon S3 can perform the associated action.</summary>
        public int NonCurrentDays { get; }

        /// <summary>The class of storage used to store the object.</summary>
        public StorageClass StorageClass { get; }
    }
}