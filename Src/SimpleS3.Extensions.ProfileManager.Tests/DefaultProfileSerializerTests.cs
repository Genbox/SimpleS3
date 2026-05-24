using System.Text;
using Genbox.SimpleS3.Extensions.ProfileManager.Abstracts;
using Genbox.SimpleS3.Extensions.ProfileManager.Serializers;

namespace Genbox.SimpleS3.Extensions.ProfileManager.Tests;

public class DefaultProfileSerializerTests
{
    [Fact]
    public void SerializeRoundTripsProfileTags()
    {
        DefaultProfileSerializer serializer = new DefaultProfileSerializer();
        TestProfile profile = new TestProfile
        {
            Name = "profile",
            CreatedOn = DateTimeOffset.UtcNow,
            Location = "memory",
            KeyId = "key-id",
            AccessKey = [1, 2, 3],
            RegionCode = "region",
            ProfileVersion = 2,
            Tags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["Protector"] = "TestProtector" }
        };

        byte[] serialized = serializer.Serialize(profile);
        TestProfile deserialized = serializer.Deserialize<TestProfile>(serialized);

        Assert.Equal(2, deserialized.ProfileVersion);
        Assert.Equal("TestProtector", deserialized.GetTag("Protector"));
        Assert.Contains("Tag.Protector=TestProtector", Encoding.UTF8.GetString(serialized), StringComparison.Ordinal);
    }

    private sealed class TestProfile : IProfile
    {
        public string Name { get; set; } = null!;
        public string KeyId { get; set; } = null!;
        public byte[] AccessKey { get; set; } = null!;
        public string RegionCode { get; set; } = null!;
        public string Location { get; set; } = null!;
        public DateTimeOffset CreatedOn { get; set; }
        public int ProfileVersion { get; set; }
        public IDictionary<string, string>? Tags { get; set; }

        public string? GetTag(string key)
        {
            if (Tags == null)
                return null;

            return Tags.TryGetValue(key, out string? value) ? value : null;
        }
    }
}