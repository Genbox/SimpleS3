using System.Text.Json;
using Genbox.SimpleS3.Core.Internals.Helpers;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class JsonHelperTests
{
    [Fact]
    public void EscapeSpecialChars()
    {
        Dictionary<string, string> pairs = new Dictionary<string, string>(StringComparer.Ordinal);
        pairs.Add("\b \f \n \r \t \" \\ /", "value");

        Assert.Equal(pairs, JsonSerializer.Deserialize<Dictionary<string, string>>(JsonHelper.EncodeJson(pairs)));

        pairs.Clear();

        pairs.Add("key", "\b \f \n \r \t \" \\ /");
        Assert.Equal(pairs, JsonSerializer.Deserialize<Dictionary<string, string>>(JsonHelper.EncodeJson(pairs)));
    }

    [Fact]
    public void GenericTest()
    {
        Dictionary<string, string> pairs = new Dictionary<string, string>(StringComparer.Ordinal);
        pairs.Add("Key", "Value");
        Assert.Equal("{\"Key\":\"Value\"}", JsonHelper.EncodeJson(pairs));

        pairs.Add("OtherKey", "OtherValue");
        Assert.Equal("{\"Key\":\"Value\",\"OtherKey\":\"OtherValue\"}", JsonHelper.EncodeJson(pairs));
    }
}