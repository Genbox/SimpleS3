using System.Collections.Generic;
using System.Linq;
using Genbox.SimpleS3.Core.Internal.Helpers;
using Xunit;

namespace Genbox.SimpleS3.Tests.Tests
{
    public class UrlHelperTests
    {
        [Fact]
        public void CreateQueryString()
        {
            Assert.Equal("", UrlHelper.CreateQueryString(Enumerable.Empty<KeyValuePair<string, string>>()));
            Assert.Equal("key=value", UrlHelper.CreateQueryString(new[] {new KeyValuePair<string, string>("key", "value")}));
            Assert.Equal("KeY=ValuE", UrlHelper.CreateQueryString(new[] {new KeyValuePair<string, string>("KeY", "ValuE")}));
            Assert.Equal("key=value&key2=value2", UrlHelper.CreateQueryString(new[] {new KeyValuePair<string, string>("key", "value"), new KeyValuePair<string, string>("key2", "value2")}));
            Assert.Equal("key", UrlHelper.CreateQueryString(new[] {new KeyValuePair<string, string>("key", null)}));
            Assert.Equal("key=", UrlHelper.CreateQueryString(new[] {new KeyValuePair<string, string>("key", string.Empty)}, outputEqualOnEmpty: true));
            Assert.Equal("key=%3D", UrlHelper.CreateQueryString(new[] {new KeyValuePair<string, string>("key", "=")}));
            Assert.Equal("key==", UrlHelper.CreateQueryString(new[] {new KeyValuePair<string, string>("key", "=")}, false));
        }

        [Fact]
        public void UrlEncode()
        {
            Assert.Equal("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._~", UrlHelper.UrlEncode("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._~"));
            Assert.Equal("%20", UrlHelper.UrlEncode(" "));
            Assert.Equal("", UrlHelper.UrlEncode(""));
        }
    }
}