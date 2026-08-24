namespace Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

/// <summary>Provides the <c>IStorage</c> API.</summary>
public interface IStorage
{
    /// <summary>Provides the <c>Get</c> API.</summary>
    byte[]? Get(string name);
    /// <summary>Provides the <c>Put</c> API.</summary>
    string Put(string name, byte[] data, bool forceOverwrite = false);
    /// <summary>Provides the <c>List</c> API.</summary>
    IEnumerable<string> List();
}