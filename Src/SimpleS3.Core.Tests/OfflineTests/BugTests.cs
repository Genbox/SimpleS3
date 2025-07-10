using Genbox.SimpleS3.GenericS3.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.Tests.OfflineTests;

public class BugTests
{
    [Fact]
    public void GitHubIssue67()
    {
        // https://github.com/Genbox/SimpleS3/issues/67
        // This tests if using the DI system interferes with existing HttpClients
        ServiceCollection collection = new ServiceCollection();
        collection.AddHttpClient();
        collection.AddGenericS3();

        using ServiceProvider provider = collection.BuildServiceProvider();
        IEnumerable<HttpClient> clients = provider.GetServices<HttpClient>();
        HttpClient client = Assert.Single(clients); //We should only have the HttpClient we added ourselves. SimpleS3 should only register HttpActions for a named HttpClientFactory.

        Assert.Empty(client.DefaultRequestHeaders); //SimpleS3 sets a few headers. They must not be present on the user's HttpClient.
    }
}