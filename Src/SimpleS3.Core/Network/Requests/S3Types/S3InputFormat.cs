using Genbox.SimpleS3.Core.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Requests.S3Types;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors | ImplicitUseTargetFlags.WithMembers)]
public abstract class S3InputFormat
{
    public CompressionType CompressionType { get; set; }

    internal abstract void Reset();
}