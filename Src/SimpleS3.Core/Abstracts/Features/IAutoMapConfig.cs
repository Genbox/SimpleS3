namespace Genbox.SimpleS3.Core.Abstracts.Features;

/// <summary>Provides the <c>IAutoMapConfig</c> API.</summary>
public interface IAutoMapConfig
{
    /// <summary>Provides the <c>AutoMapDisabledFor</c> API.</summary>
    Func<Type, bool> AutoMapDisabledFor { get; }
}