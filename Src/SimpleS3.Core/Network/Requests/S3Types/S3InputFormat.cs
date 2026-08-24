using Genbox.SimpleS3.Core.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

/// <summary>Provides the <c>S3InputFormat</c> API.</summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors | ImplicitUseTargetFlags.WithMembers)]
public abstract class S3InputFormat
{
    /// <summary>Provides the <c>CompressionType</c> API.</summary>
    public CompressionType CompressionType { get; set; }

    internal virtual void Reset()
    {
        CompressionType = CompressionType.Unknown;
    }
}