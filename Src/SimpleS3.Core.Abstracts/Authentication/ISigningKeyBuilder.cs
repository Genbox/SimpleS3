namespace Genbox.SimpleS3.Core.Abstracts.Authentication;

/// <summary>Provides the <c>ISigningKeyBuilder</c> API.</summary>
public interface ISigningKeyBuilder
{
    /// <summary>Provides the <c>CreateSigningKey</c> API.</summary>
    byte[] CreateSigningKey(DateTimeOffset dateTime);
}