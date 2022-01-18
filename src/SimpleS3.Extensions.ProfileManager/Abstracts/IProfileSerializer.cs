namespace Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

public interface IProfileSerializer
{
    byte[] Serialize<T>(T profile) where T : IProfile;
    T Deserialize<T>(byte[] data) where T : IProfile;
}