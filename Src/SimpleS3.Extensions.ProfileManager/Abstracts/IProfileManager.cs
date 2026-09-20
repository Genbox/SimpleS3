namespace Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

/// <summary>Provides the <c>IProfileManager</c> API.</summary>
public interface IProfileManager
{
    /// <summary>Provides the <c>GetProfile</c> API.</summary>
    IProfile? GetProfile(string name);

    /// <summary>Provides the <c>SaveProfile</c> API.</summary>
    string SaveProfile(IProfile profile);

    /// <summary>Provides the <c>CreateProfile</c> API.</summary>
    IProfile CreateProfile(string name, string keyId, byte[] accessKey, string region, out string? location, bool persist = true);

    /// <summary>Provides the <c>List</c> API.</summary>
    IEnumerable<IProfile> List();
}