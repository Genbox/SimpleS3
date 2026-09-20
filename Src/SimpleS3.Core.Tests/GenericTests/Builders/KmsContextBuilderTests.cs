using System.Text.Json;
using Genbox.SimpleS3.Core.Builders;

namespace Genbox.SimpleS3.Core.Tests.GenericTests.Builders;

public class KmsContextBuilderTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ContextKeysAreCaseSensitive(bool fromDictionary)
    {
        Dictionary<string, string> expected = new Dictionary<string, string>(StringComparer.Ordinal) { ["Key"] = "one", ["key"] = "two" };
        KmsContextBuilder builder = fromDictionary ? new KmsContextBuilder(expected) : new KmsContextBuilder();

        if (!fromDictionary)
        {
            foreach (KeyValuePair<string, string> pair in expected)
                builder.AddEntry(pair.Key, pair.Value);
        }

        using JsonDocument json = JsonDocument.Parse(builder.Build()!);
        Assert.Equal("one", json.RootElement.GetProperty("Key").GetString());
        Assert.Equal("two", json.RootElement.GetProperty("key").GetString());
        Assert.Equal(2, JsonSerializer.Deserialize<Dictionary<string, string>>(builder.Build()!)!.Count);
        Assert.Throws<ArgumentException>(() => builder.AddEntry("Key", "replacement"));
        Assert.Equal("one", JsonSerializer.Deserialize<Dictionary<string, string>>(builder.Build()!)!["Key"]);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ContextStringsRoundTripThroughJson(bool fromDictionary)
    {
        string controls = new string(Enumerable.Range(0, 32).Select(x => (char)x).ToArray());
        Dictionary<string, string> expected = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["quoted\" key\\/"] = "spaces ! \u00a4 \u6771\u4eac \ud83d\ude00",
            [controls] = controls,
            ["empty"] = string.Empty
        };
        KmsContextBuilder builder = fromDictionary ? new KmsContextBuilder(expected) : new KmsContextBuilder();

        if (!fromDictionary)
        {
            foreach (KeyValuePair<string, string> pair in expected)
                builder.AddEntry(pair.Key, pair.Value);
        }

        Assert.Equal(expected, JsonSerializer.Deserialize<Dictionary<string, string>>(builder.Build()!));
    }

    [Fact]
    public void GenericTest()
    {
        KmsContextBuilder b = new KmsContextBuilder();
        Assert.Null(b.Build());

        b.AddEntry("SomeKey", "SomeValue");

        Assert.Equal("{\"SomeKey\":\"SomeValue\"}", b.Build());

        b.AddEntry("SomeOtherKey", "SomeOtherValue");

        Assert.Equal("{\"SomeKey\":\"SomeValue\",\"SomeOtherKey\":\"SomeOtherValue\"}", b.Build());
    }
}