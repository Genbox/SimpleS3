namespace Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

public interface IProfile
{
    string Name { get; }
    string KeyId { get; }
    byte[] AccessKey { get; }
    string RegionCode { get; }
    string Location { get; }
    DateTimeOffset CreatedOn { get; }
    IDictionary<string, string>? Tags { get; }
    string? GetTag(string key);
}