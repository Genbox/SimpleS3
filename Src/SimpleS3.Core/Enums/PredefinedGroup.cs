﻿using System.ComponentModel.DataAnnotations;
using Genbox.FastEnum;

namespace Genbox.SimpleS3.Core.Enums;

[FastEnum]
public enum PredefinedGroup
{
    Unknown = 0,

    /// <summary>
    /// Access permission to this group allows anyone in the world access to the resource. The requests can be signed (authenticated) or unsigned (anonymous). Unsigned requests
    /// omit the Authentication header in the request.
    /// </summary>
    [Display(Name = "http://acs.amazonaws.com/groups/global/AllUsers")]
    AllUsers,

    /// <summary>
    /// his group represents all AWS accounts. Access permission to this group allows any AWS account to access the resource. However, all requests must be signed
    /// (authenticated).
    /// </summary>
    [Display(Name = "http://acs.amazonaws.com/groups/global/AuthenticatedUsers")]
    AuthenticatedUsers,

    /// <summary>WRITE permission on a bucket enables this group to write server access logs (see Amazon S3 Server Access Logging) to the bucket.</summary>
    [Display(Name = "http://acs.amazonaws.com/groups/s3/LogDelivery")]
    LogDelivery
}