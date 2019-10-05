namespace Genbox.SimpleS3.Abstracts.Authentication
{
    public interface IAccessKey
    {
        string KeyId { get; }
        byte[] AccessKey { get; }
    }
}