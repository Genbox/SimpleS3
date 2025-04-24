using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Misc;

public class MiscTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task IsEncodingCorrect(S3Provider _, string bucket, ISimpleClient client)
    {
        //This tests if '=' is NOT encoded. S3 does not use encoding of paths in signed requests, but all other AWS APIs do
        var putResp = await client.PutObjectStringAsync(bucket, "a=a", "hello");
        Assert.Equal(200, putResp.StatusCode);
    }
}