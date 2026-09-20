using Genbox.SimpleS3.Core.Common.Helpers;

namespace Genbox.SimpleS3.Core.Common.Tests;

public class UrlHelperTests
{
    [Theory]
    [InlineData(".", "%2E")]
    [InlineData("..", "%2E%2E")]
    [InlineData("...", "...")]
    [InlineData("....", "....")]
    [InlineData("a/../..././b", "a/%2E%2E/.../%2E/b")]
    public void PathEncodingOnlyEscapesNavigationDotSegments(string key, string expected)
    {
        Assert.Equal(expected, UrlHelper.UrlPathEncode(key));
        Uri uri = UrlHelper.CreateUriWithoutPathNormalization("https://example.com/" + expected);
        Assert.Equal("/" + expected, uri.AbsolutePath);
    }
}