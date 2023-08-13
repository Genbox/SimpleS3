using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Internal;

internal class Profile : IProfile
{
    public IDictionary<string, string>? Tags { get; private set; }

    public string Name { get; internal set; } = null!;
    public string KeyId { get; internal set; } = null!;
    public byte[] AccessKey { get; internal set; } = null!;
    public string RegionCode { get; internal set; } = null!;
    public string Location { get; internal set; } = null!;

    public DateTimeOffset CreatedOn { get; internal set; }

    public string? GetTag(string key)
    {
        if (Tags == null)
            return null;

        return Tags.TryGetValue(key, out string? value) ? value : null;
    }

    public void AddTag(string key, string value)
    {
        Tags ??= new Dictionary<string, string>(1, StringComparer.OrdinalIgnoreCase);
        Tags.Add(key, value);
    }
}