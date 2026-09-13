using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

/// <summary>The namespace in which a bucket name is allocated.</summary>
[FastEnum]
public enum BucketNamespace
{
    /// <summary>Leave namespace selection to the service.</summary>
    Unknown = 0,

    /// <summary>The global bucket namespace.</summary>
    [Display(Name = "global")]
    Global,

    /// <summary>The namespace scoped to the account and region.</summary>
    [Display(Name = "account-regional")]
    AccountRegional
}