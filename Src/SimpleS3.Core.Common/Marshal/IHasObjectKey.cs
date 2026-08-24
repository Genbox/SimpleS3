namespace Genbox.SimpleS3.Core.Common.Marshal;

/// <summary>Provides the <c>IHasObjectKey</c> API.</summary>
public interface IHasObjectKey
{
    /// <summary>Provides the <c>ObjectKey</c> API.</summary>
    string ObjectKey { get; }
}