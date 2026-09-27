using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Clients;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Abstracts.Transfer;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Genbox.SimpleS3.Core.TestBase.Code;
using Genbox.SimpleS3.ProviderBase;
using Microsoft.Extensions.DependencyInjection;

namespace Genbox.SimpleS3.Core.TestBase;

public abstract class ProviderClientTestBase<TClient> where TClient : class, ISimpleClient, IDisposable
{
    [SuppressMessage("Design", "CA1056", Justification = "Endpoint assertions compare the exact request URL text captured by the test driver.")]
    protected abstract string ExpectedObjectUrl { get; }

    protected abstract TClient CreateClient(NullNetworkDriver driver);
    protected abstract TClient CreateClient(NetworkConfig? networkConfig);
    protected abstract void ConfigureServices(IServiceCollection services);

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void StandaloneClientBuildsWithDefaultOrCustomNetworkConfiguration(bool useCustomConfiguration)
    {
        using TClient client = CreateClient(useCustomConfiguration ? new NetworkConfig { Retries = 1, Timeout = TimeSpan.FromSeconds(10) } : null);

        string url = client.SignRequest(new GetObjectRequest("testbucket", "GetObjectAsync"), TimeSpan.FromMinutes(5));

        Assert.StartsWith(ExpectedObjectUrl + "?", url, StringComparison.Ordinal);
        Assert.Contains("X-Amz-Signature=", url, StringComparison.Ordinal);
    }

    [Fact]
    public async Task StandaloneClientDoesNotDisposeSuppliedDriverAsync()
    {
        using DisposableNetworkDriver driver = new DisposableNetworkDriver();

        using (TClient client = CreateClient(driver))
        {
            using GetObjectResponse response = await client.GetObjectAsync("testbucket", "GetObjectAsync", token: TestContext.Current.CancellationToken);
            Assert.Equal(200, response.StatusCode);
        }

        Assert.False(driver.IsDisposed);
    }

    [Fact]
    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP007", Justification = "Verifies that disposing the DI client does not dispose the container-owned service graph.")]
    public async Task DisposingDependencyInjectedClientLeavesContainerServicesUsableAsync()
    {
        using DisposableNetworkDriver driver = new DisposableNetworkDriver();
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton<INetworkDriver>(driver);
        ConfigureServices(services);
        await using ServiceProvider provider = services.BuildServiceProvider();

        TClient client = provider.GetRequiredService<TClient>();
        client.Dispose();

        using GetObjectResponse response = await provider.GetRequiredService<ISimpleClient>().GetObjectAsync("testbucket", "GetObjectAsync", token: TestContext.Current.CancellationToken);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal(ExpectedObjectUrl, driver.LastUrl);
        Assert.False(driver.IsDisposed);
    }

    [Fact]
    public async Task GetObjectUsesProviderEndpointAsync()
    {
        NullNetworkDriver driver = new NullNetworkDriver();
        using TClient client = CreateClient(driver);

        using GetObjectResponse response = await client.GetObjectAsync("testbucket", "GetObjectAsync", token: TestContext.Current.CancellationToken);

        Assert.Equal(200, response.StatusCode);
        Assert.Equal(ExpectedObjectUrl, driver.LastUrl);
    }

    [Fact]
    public async Task ObjectClientInterfaceReturnsVersionsAsync()
    {
        NullNetworkDriver driver = new NullNetworkDriver
        {
            ResponseContent = "<ListVersionsResult><Version><Key>key</Key><VersionId>123</VersionId></Version></ListVersionsResult>"
        };
        using TClient client = CreateClient(driver);

        ListObjectVersionsResponse response = await client.ListObjectVersionsAsync("testbucket", token: TestContext.Current.CancellationToken);

        Assert.Equal(200, response.StatusCode);
        Assert.Equal("123", Assert.Single(response.Versions).VersionId);
    }

    [Fact]
    public void EveryInterfaceOperationMapsToPublicMethodOrThrowingFallback()
    {
        Type clientType = typeof(TClient);
        MethodInfo[] publicMethods = clientType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        foreach (Type contract in typeof(ISimpleClient).GetInterfaces().Append(typeof(ISimpleClient)))
        {
            InterfaceMapping mapping = clientType.GetInterfaceMap(contract);

            for (int i = 0; i < mapping.InterfaceMethods.Length; i++)
            {
                MethodInfo operation = mapping.InterfaceMethods[i];
                MethodInfo target = mapping.TargetMethods[i];

                // Close generic signatures before comparing interface and class parameter types.
                MethodInfo signature = CloseGenericMethod(operation);
                MethodInfo? publicMethod = publicMethods.SingleOrDefault(method =>
                    method.Name == operation.Name &&
                    method.GetGenericArguments().Length == operation.GetGenericArguments().Length &&
                    CloseGenericMethod(method).GetParameters().Select(parameter => parameter.ParameterType)
                                              .SequenceEqual(signature.GetParameters().Select(parameter => parameter.ParameterType)));

                if (publicMethod != null)
                {
                    Assert.Equal(publicMethod, target);
                    Assert.Equal(signature.ReturnType, CloseGenericMethod(target).ReturnType);
                    Assert.Equal(operation.GetParameters().Select(parameter => parameter.DefaultValue), publicMethod.GetParameters().Select(parameter => parameter.DefaultValue));
                }
                else
                {
                    Assert.Equal(typeof(ProviderClientAdapter), target.DeclaringType);
                    Assert.False(target.IsPublic);
                }
            }
        }
    }

    [Fact]
    public void DependencyInjectionSharesConcreteAndCommonClient()
    {
        ServiceCollection services = new ServiceCollection();
        ConfigureServices(services);
        using ServiceProvider provider = services.BuildServiceProvider();

        TClient client = provider.GetRequiredService<TClient>();
        Assert.Same(client, provider.GetRequiredService<TClient>());
        Assert.Same(client, provider.GetRequiredService<ISimpleClient>());
    }

    [Theory]
    [InlineData(typeof(IBucketClient))]
    [InlineData(typeof(IObjectClient))]
    [InlineData(typeof(IMultipartClient))]
    [InlineData(typeof(ISignedClient))]
    [InlineData(typeof(ITransfer))]
    [InlineData(typeof(IMultipartTransfer))]
    public void DependencyInjectionSharesProviderAcrossDomainInterfaces(Type contract)
    {
        Assert.NotNull(contract);
        ServiceCollection services = new ServiceCollection();
        ConfigureServices(services);
        using ServiceProvider provider = services.BuildServiceProvider();

        TClient client = provider.GetRequiredService<TClient>();
        Assert.True(contract.IsInstanceOfType(client));
        Assert.Same(client, provider.GetRequiredService(contract));
    }

    private static MethodInfo CloseGenericMethod(MethodInfo method)
    {
        if (!method.IsGenericMethodDefinition)
            return method;

        return method.MakeGenericMethod(method.Name == nameof(ISignedClient.SignRequest) ? typeof(GetObjectRequest) : typeof(GetObjectResponse));
    }

    private sealed class DisposableNetworkDriver : NullNetworkDriver, IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose() => IsDisposed = true;
    }
}