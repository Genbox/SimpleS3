namespace Genbox.SimpleS3.Core.Abstracts.Authentication
{
    public interface IAccessKey
    {
        string KeyId { get; }
        byte[] AccessKey { get; }
    }
}