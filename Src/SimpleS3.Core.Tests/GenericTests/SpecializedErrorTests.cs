using Genbox.SimpleS3.Core.Network.Responses.Errors;

namespace Genbox.SimpleS3.Core.Tests.GenericTests;

public class SpecializedErrorTests
{
    [Theory]
    [InlineData("AccessDenied", false)]
    [InlineData("FutureProviderError", true)]
    public void ConstructorCopiesDetailsWithoutMutatingCaller(string code, bool unknown)
    {
        Dictionary<string, string> input = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Code"] = code,
            ["Message"] = "original message",
            ["RequestId"] = "original request"
        };
        GenericError error = new GenericError(input);
        Assert.Equal(3, input.Count);
        Assert.Equal(code, input["Code"]);
        Assert.Equal("original message", input["Message"]);
        Assert.Equal("original request", input["RequestId"]);
        input["Code"] = "changed";
        input["Message"] = "changed";
        input["RequestId"] = "changed";
        Assert.Equal(unknown ? "Unknown" : code, error.Code.ToString());
        Assert.Equal("original message", error.Message);
        Assert.Equal("original request", error.Data["requestid"]);
        if (unknown)
            Assert.Equal(code, error.Data["Code"]);
        else
            Assert.False(error.Data.ContainsKey("Code"));
        error.Data["RequestId"] = "error-owned";
        error.Data["Extra"] = "value";
        Assert.Equal("changed", input["RequestId"]);
        Assert.False(input.ContainsKey("Extra"));
    }
}