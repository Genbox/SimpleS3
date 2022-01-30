using Genbox.SimpleS3.Core.Common.Validation;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public class S3Expiration
{
    /// <summary>Set a future date where the objects should expire.</summary>
    /// <param name="expireOn">Indicates at what date the object is to be moved or deleted</param>
    public S3Expiration(DateTimeOffset expireOn)
    {
        ExpireOnDate = expireOn;
    }

    /// <summary>Indicates the lifetime, in days, of the objects that are subject to the rule. The value must be a non-zero
    /// positive integer.</summary>
    /// <param name="expireAfterDays">A non-zero positive integer</param>
    public S3Expiration(int expireAfterDays)
    {
        Validator.RequireThat(expireAfterDays > 0, nameof(expireAfterDays));

        ExpireAfterDays = expireAfterDays;
    }

    /// <summary>Indicates whether Amazon S3 will remove a delete marker with no noncurrent versions.</summary>
    /// <param name="expireDeleteMarker">If set to true, the delete marker will be expired; if set to false the policy takes no
    /// action.</param>
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

    public DateTimeOffset? ExpireOnDate { get; }
    public int? ExpireAfterDays { get; }
    public bool? ExpireObjectDeleteMarker { get; }
}