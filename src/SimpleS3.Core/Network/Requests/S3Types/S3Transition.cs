using System;
using System.Transactions;
using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types
{
    /// <summary>Specifies when an object transitions to a specified storage class.</summary>
    public class S3Transition
    {
        /// <summary>Set a future date where the objects should transition.</summary>
        /// <param name="date">Indicates at what date the object is transitioned</param>
        /// <param name="storageClass">The storage class to which you want the object to transition.</param>
        public S3Transition(DateTimeOffset date, StorageClass storageClass)
        {
            Validator.RequireThat(storageClass != StorageClass.Unknown, nameof(storageClass));

            TransitionOnDate = date;
            StorageClass = storageClass;
        }

        /// <summary>Indicates the number of days after creation when objects are transitioned to the specified storage class.</summary>
        /// <param name="transitionAfterDays">The value must be a positive integer.</param>
        /// <param name="storageClass">The storage class to which you want the object to transition.</param>
        public S3Transition(int transitionAfterDays, StorageClass storageClass)
        {
            Validator.RequireThat(transitionAfterDays > 0, nameof(transitionAfterDays));
            Validator.RequireThat(storageClass != StorageClass.Unknown, nameof(storageClass));

            TransitionAfterDays = transitionAfterDays;
            StorageClass = storageClass;
        }

        internal S3Transition(DateTimeOffset? transitionOnDate, int? transitionAfterDays, StorageClass storageClass)
        {
            TransitionOnDate = transitionOnDate;
            TransitionAfterDays = transitionAfterDays;
            StorageClass = storageClass;
        }

        public DateTimeOffset? TransitionOnDate { get; }
        public int? TransitionAfterDays { get; }
        public StorageClass StorageClass { get; }
    }
}