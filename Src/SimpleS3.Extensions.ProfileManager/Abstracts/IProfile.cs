namespace Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

public interface IProfile
{
    string Name { get; set; }
    string KeyId { get; set; }
    byte[] AccessKey { get; set; }
    string RegionCode { get; set; }
    string Location { get; set; }
    DateTimeOffset CreatedOn { get; set; }
    IDictionary<string, string>? Tags { get; set; }
    string? GetTag(string key);
}