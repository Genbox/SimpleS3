using Genbox.SimpleS3.Core.Common.Validation;
using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Specifies when an object transitions to a specified storage class.</summary>
public class S3Transition
{
    /// <summary>Set a future date where the objects should transition.</summary>
    /// <param name="date">The transition date. S3 request serialization converts this value to UTC and uses midnight at the start of that UTC calendar day. The supplied value is retained in this object.</param>
    /// <param name="storageClass">The storage class to which you want the object to transition.</param>
    public S3Transition(DateTimeOffset date, StorageClass storageClass)
    {
        Validator.RequireThat(Enums.Enums.StorageClass.IsDefined(storageClass));

        TransitionOnDate = date;
        StorageClass = storageClass;
    }

    /// <summary>Indicates the number of days after creation when objects are transitioned to the specified storage class.</summary>
    /// <param name="transitionAfterDays">The value must be zero or a positive integer.</param>
    /// <param name="storageClass">The storage class to which you want the object to transition.</param>
    public S3Transition(int transitionAfterDays, StorageClass storageClass)
    {
        Validator.RequireThat(transitionAfterDays >= 0);
        Validator.RequireThat(Enums.Enums.StorageClass.IsDefined(storageClass));

        TransitionAfterDays = transitionAfterDays;
        StorageClass = storageClass;
    }

    internal S3Transition(DateTimeOffset? transitionOnDate, int? transitionAfterDays, StorageClass storageClass)
    {
        TransitionOnDate = transitionOnDate;
        TransitionAfterDays = transitionAfterDays;
        StorageClass = storageClass;
    }

    /// <summary>Provides the <c>TransitionOnDate</c> API.</summary>
    public DateTimeOffset? TransitionOnDate { get; }

    /// <summary>Provides the <c>TransitionAfterDays</c> API.</summary>
    public int? TransitionAfterDays { get; }

    /// <summary>Provides the <c>StorageClass</c> API.</summary>
    public StorageClass StorageClass { get; }
}