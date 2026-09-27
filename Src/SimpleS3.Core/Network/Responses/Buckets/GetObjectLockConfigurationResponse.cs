using Genbox.SimpleS3.Core.Enums;

namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Provides the <c>GetObjectLockConfigurationResponse</c> API.</summary>
public class GetObjectLockConfigurationResponse : BaseResponse
{
    /// <summary>Whether object lock is enabled for the bucket.</summary>
    public bool Enabled { get; internal set; }

    /// <summary>Provides the <c>LockMode</c> API.</summary>
    public LockMode LockMode { get; internal set; }

    /// <summary>Default retention duration in days, when returned by the service.</summary>
    public int? LockRetainDays { get; internal set; }

    /// <summary>Default retention duration in calendar years, when returned by the service.</summary>
    public int? LockRetainYears { get; internal set; }
}