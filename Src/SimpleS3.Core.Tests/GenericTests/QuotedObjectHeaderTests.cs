using Genbox.SimpleS3.Core.Internals.Helpers;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class QuotedObjectHeaderTests
{
    [Theory]
    [InlineData("expiry-date=\"Fri, 21 Dec 2012 00:00:00 GMT\", rule-id=\"a%2Fb+c\"")]
    [InlineData(" RULE-ID = \"a%2Fb+c\" , EXPIRY-DATE = \"Fri, 21 Dec 2012 00:00:00 GMT\" ")]
    [InlineData("extension=\"a,b\", rule-id=\"a%2Fb+c\", expiry-date=\"Fri, 21 Dec 2012 00:00:00 GMT\"")]
    public void ExpirationPreservesQuotedCommasAndDecodesRuleId(string header)
    {
        Assert.True(ParserHelper.TryParseExpiration(new Dictionary<string, string>(StringComparer.Ordinal) { ["x-amz-expiration"] = header }, out (DateTimeOffset expiresOn, string ruleId) data));
        Assert.Equal(new DateTimeOffset(2012, 12, 21, 0, 0, 0, TimeSpan.Zero), data.expiresOn);
        Assert.Equal("a/b c", data.ruleId);
    }
}