using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Issues;

public class BackBlazeIssues : TestBase
{
    /// <summary>https://github.com/Genbox/SimpleS3/issues/55</summary>
    [Theory]
    [SingleProvider(S3Provider.BackBlazeB2)]
    public async Task Issue55(S3Provider _, string bucket, ISimpleClient client)
    {
        //Issue: When uploading 460+ KB to BackBlaze, it gives an exception with "error occured while sending the request".
        //Investigation: BackBlaze does not support the 2MB chunk size that I defaulted to in SimpleS3Config.
        //Resolution: I changed the default chunk size to be 80 KB. The same as what Amazon use for their clients.

        byte[] bytes = new byte[5 * 1024 * 1024];
        Random.Shared.NextBytes(bytes);
        PutObjectResponse resp = await client.PutObjectDataAsync(bucket, "Technical Details.pdf", bytes);
        Assert.True(resp.IsSuccess);
    }
}