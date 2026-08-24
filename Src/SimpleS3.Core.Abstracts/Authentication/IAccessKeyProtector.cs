namespace Genbox.SimpleS3.Core.Abstracts.Authentication;

/// <summary>Provides the <c>IAccessKeyProtector</c> API.</summary>
public interface IAccessKeyProtector
{
    /// <summary>Provides the <c>ProtectKey</c> API.</summary>
    byte[] ProtectKey(byte[] key);
    /// <summary>Provides the <c>UnprotectKey</c> API.</summary>
    byte[] UnprotectKey(byte[] key);
}