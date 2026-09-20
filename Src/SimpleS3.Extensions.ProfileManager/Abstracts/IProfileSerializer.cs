namespace Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

/// <summary>Provides the <c>IProfileSerializer</c> API.</summary>
public interface IProfileSerializer
{
    /// <summary>Provides the <c>Serialize&lt;T&gt;</c> API.</summary>
    byte[] Serialize<T>(T profile) where T : IProfile;

    /// <summary>Provides the <c>Deserialize&lt;T&gt;</c> API.</summary>
    T Deserialize<T>(byte[] data) where T : IProfile;
}