namespace Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

public interface IStorage
{
    byte[]? Get(string name);
    string Put(string name, byte[] data);
    IEnumerable<string> List();
}