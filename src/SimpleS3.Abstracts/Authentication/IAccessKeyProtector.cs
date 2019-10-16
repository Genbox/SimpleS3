namespace Genbox.SimpleS3.Abstracts.Authentication
{
    public interface IAccessKeyProtector
    {
        byte[] ProtectKey(byte[] key);
        byte[] UnprotectKey(byte[] key);
    }
}