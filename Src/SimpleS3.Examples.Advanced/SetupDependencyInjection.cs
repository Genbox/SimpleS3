using Genbox.SimpleS3.AmazonS3.Extensions;
using Genbox.SimpleS3.Core.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Examples.Advanced;

public static class SetupDependencyInjection
{
    /// <summary>Shows how to use dependency injection to setup a client to AmazonS3</summary>
    public static void Example()
    {
        //Create the dependency injection container
        ServiceCollection services = new ServiceCollection();

        //Add all the AmazonS3 services
        services.AddAmazonS3();

        //Build the service provider
        using ServiceProvider serviceProvider = services.BuildServiceProvider();

        //Now we can get the client.
        ISimpleClient client = serviceProvider.GetRequiredService<ISimpleClient>();
    }
}