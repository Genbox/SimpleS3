using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.TestBase.Code;
using Genbox.SimpleS3.GenericS3;
using Genbox.SimpleS3.GenericS3.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Genbox.SimpleS3.Extensions.GenericS3.Tests;

public class GenericS3IssueTests
{
    [Fact]
    public async Task Issue59Test()
    {
        //This test tries to replicate a failure where RegionData is not injected into DI when using GenericS3Client

        ServiceCollection services = new ServiceCollection();

        services.AddGenericS3(c =>
        {
            string endpoint = "http://domain.internal";
            string accessKey = "mykey";
            string secretKey = "secretkey";
            string region = "eu-west-1";

            c.Endpoint = endpoint;
            c.Credentials = new StringAccessKey(accessKey, secretKey);
            c.RegionCode = region;
            c.NamingMode = NamingMode.PathStyle;
        });

        //Replace the network driver with the null one for testing
        services.Replace(ServiceDescriptor.Singleton<INetworkDriver, NullNetworkDriver>());

        ServiceProvider provider = services.BuildServiceProvider();

        GenericS3Client client = provider.GetRequiredService<GenericS3Client>();
        GetObjectResponse resp = await client.GetObjectAsync("bucket", "object");
        Assert.True(resp.IsSuccess);
    }
}