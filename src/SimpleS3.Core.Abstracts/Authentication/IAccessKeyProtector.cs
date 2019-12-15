namespace Genbox.SimpleS3.Core.Abstracts.Authentication
{
    public interface IAccessKeyProtector
    {
        byte[] ProtectKey(byte[] key);
        byte[] UnprotectKey(byte[] key);
    }
}