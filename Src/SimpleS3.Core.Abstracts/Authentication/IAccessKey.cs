namespace Genbox.SimpleS3.Core.Abstracts.Authentication;

/// <summary>Provides the <c>IAccessKey</c> API.</summary>
public interface IAccessKey
{
    /// <summary>Provides the <c>KeyId</c> API.</summary>
    string KeyId { get; }
    /// <summary>Provides the <c>SecretKey</c> API.</summary>
    byte[] SecretKey { get; }
}