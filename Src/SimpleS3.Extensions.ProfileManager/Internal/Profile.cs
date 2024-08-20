using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Internal;

internal sealed class Profile : IProfile
{
    [UsedImplicitly]
    public Profile() {}

    public IDictionary<string, string>? Tags { get; set; }

    public string Name { get; set; } = null!;
    public string KeyId { get; set; } = null!;
    public byte[] AccessKey { get; set; } = null!;
    public string RegionCode { get; set; } = null!;
    public string Location { get; set; } = null!;
    public DateTimeOffset CreatedOn { get; set; }

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