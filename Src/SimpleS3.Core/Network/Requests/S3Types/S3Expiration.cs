using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3Expiration</c> API.</summary>
public class S3Expiration
{
    /// <summary>Set a future date where the objects should expire.</summary>
    /// <param name="expireOn">The expiration date. S3 request serialization converts this value to UTC and uses midnight at the start of that UTC calendar day. The supplied value is retained in this object.</param>
    public S3Expiration(DateTimeOffset expireOn)
    {
        ExpireOnDate = expireOn;
    }

    /// <summary>Indicates the lifetime, in days, of the objects that are subject to the rule. The value must be a non-zero positive integer.</summary>
    /// <param name="expireAfterDays">A non-zero positive integer</param>
    public S3Expiration(int expireAfterDays)
    {
        Validator.RequireThat(expireAfterDays > 0);

        ExpireAfterDays = expireAfterDays;
    }

    /// <summary>Indicates whether Amazon S3 will remove a delete marker with no noncurrent versions.</summary>
    /// <param name="expireDeleteMarker">If set to true, the delete marker will be expired; if set to false the policy takes no action.</param>
    public S3Expiration(bool expireDeleteMarker)
    {
        ExpireObjectDeleteMarker = expireDeleteMarker;
    }

    internal S3Expiration(DateTimeOffset? expireOnDate, int? expireAfterDays, bool? expireObjectDeleteMarker)
    {
        ExpireOnDate = expireOnDate;
        ExpireAfterDays = expireAfterDays;
        ExpireObjectDeleteMarker = expireObjectDeleteMarker;
    }

    /// <summary>Provides the <c>ExpireOnDate</c> API.</summary>
    public DateTimeOffset? ExpireOnDate { get; }

    /// <summary>Provides the <c>ExpireAfterDays</c> API.</summary>
    public int? ExpireAfterDays { get; }

    /// <summary>Provides the <c>ExpireObjectDeleteMarker</c> API.</summary>
    public bool? ExpireObjectDeleteMarker { get; }
}