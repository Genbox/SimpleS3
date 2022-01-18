using System.Collections.Generic;
using Genbox.SimpleS3.Core.Internals.Helpers;
using Xunit;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class JsonHelperTests
{
    [Fact]
    public void EscapeSpecialChars()
    {
        Dictionary<string, string> pairs = new Dictionary<string, string>();
        pairs.Add("\b \f \n \r \t \" \\ /", "value");

        Assert.Equal($"\"\\{(char)8} \\{(char)12} \\{'\n'} \\{'\r'} \\{'\t'} \\\" \\\\ \\/\":\"value\"", JsonHelper.EncodeJson(pairs));

        pairs.Clear();

        pairs.Add("key", "\b \f \n \r \t \" \\ /");
        Assert.Equal($"\"key\":\"\\{(char)8} \\{(char)12} \\{'\n'} \\{'\r'} \\{'\t'} \\\" \\\\ \\/\"", JsonHelper.EncodeJson(pairs));
    }

    [Fact]
    public void GenericTest()
    {
        Dictionary<string, string> pairs = new Dictionary<string, string>();
        pairs.Add("Key", "Value");
        Assert.Equal("\"Key\":\"Value\"", JsonHelper.EncodeJson(pairs));

        pairs.Add("OtherKey", "OtherValue");
        Assert.Equal("\"Key\":\"Value\",\"OtherKey\":\"OtherValue\"", JsonHelper.EncodeJson(pairs));
    }
}