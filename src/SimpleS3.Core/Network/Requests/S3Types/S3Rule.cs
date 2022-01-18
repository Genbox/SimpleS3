﻿using System.Collections.Generic;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

public class S3Rule
{
    public S3Rule(string id, bool enabled)
    {
        Id = id;
        Enabled = enabled;
        NonCurrentVersionTransitions = new List<S3NonCurrentVersionTransition>();
        Transitions = new List<S3Transition>();
    }

    /// <summary>Unique identifier for the rule. The value cannot be longer than 255 characters.</summary>
    public string Id { get; }

    /// <summary>
    /// Specifies the days since the initiation of an incomplete multipart upload that Amazon S3 will wait before permanently removing all parts of
    /// the upload.
    /// </summary>
    public int? AbortIncompleteMultipartUploadDays { get; set; }

    /// <summary>Specifies the expiration for the lifecycle of the object in the form of date, days and, whether the object has a delete marker.</summary>
    public S3Expiration? Expiration { get; set; }

    /// <summary>The Filter is used to identify objects that a Lifecycle Rule applies to. A Filter must have exactly one of Prefix, Tag, or And specified.</summary>
    public S3Filter? Filter { get; set; }

    /// <summary>Specifies the number of days an object is noncurrent before Amazon S3 can perform the associated action.</summary>
    public int? NonCurrentVersionExpirationDays { get; set; }

    /// <summary>
    /// Specifies the transition rule for the lifecycle rule that describes when noncurrent objects transition to a specific storage class. If your
    /// bucket is versioning-enabled (or versioning is suspended), you can set this action to request that Amazon S3 transition noncurrent object versions to
    /// a specific storage class at a set period in the object's lifetime.
    /// </summary>
    public IList<S3NonCurrentVersionTransition> NonCurrentVersionTransitions { get; }

    /// <summary>If true, the rule is enabled. If false, the rule is disabled.</summary>
    public bool Enabled { get; }

    /// <summary>Specifies when an Amazon S3 object transitions to a specified storage class.</summary>
    public IList<S3Transition> Transitions { get; }
}