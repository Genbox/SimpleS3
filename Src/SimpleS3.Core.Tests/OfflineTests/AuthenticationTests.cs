using System.Security.Cryptography;
using System.Text;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Abstracts.Request;
using Genbox.SimpleS3.Core.Common.Authentication;
using Genbox.SimpleS3.Core.Extensions;
using Genbox.SimpleS3.Core.Internals.Authentication;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.TestBase.Code;
using Genbox.SimpleS3.Extensions.HttpClientFactory.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Genbox.SimpleS3.Core.Tests.OfflineTests;

public class AuthenticationTests
{
    [Theory]
    [InlineData("AmazonS3")]
    [InlineData("BackBlazeB2")]
    [InlineData("Wasabi")]
    public async Task PublicHeaderSigningTracksRegionMutation(string providerName)
    {
        NullNetworkDriver network = new NullNetworkDriver();
        await using ServiceProvider provider = CreateSigningProvider(network, providerName);
        ISimpleClient client = provider.GetRequiredService<ISimpleClient>();
        SimpleS3Config config = provider.GetRequiredService<IOptions<SimpleS3Config>>().Value;
        config.Endpoint = "https://s3.{Region}.example.test";
        const string prefix = "x-amz-";
        const string algorithm = "AWS4-HMAC-SHA256";

        foreach (string region in new[] { "us-east-1", "eu-west-1", "us-east-1" })
        {
            config.RegionCode = region;
            await client.HeadObjectAsync("test", "object", token: TestContext.Current.CancellationToken);
            Assert.Equal("https://s3." + region + ".example.test/test/object", network.LastUrl);
            Assert.NotNull(network.LastHeaders);
            IReadOnlyDictionary<string, string> headers = network.LastHeaders;
            string authorization = headers["authorization"];
            Assert.StartsWith(algorithm + " Credential=key/", authorization, StringComparison.Ordinal);
            string[] fields = authorization[(algorithm.Length + 1)..].Split(',', StringSplitOptions.TrimEntries);
            Assert.Equal(3, fields.Length);
            string scope = fields[0]["Credential=key/".Length..];
            Assert.EndsWith("/" + region + "/s3/aws4_request", scope, StringComparison.Ordinal);
            string signedHeaders = fields[1]["SignedHeaders=".Length..];
            Assert.Contains("host", signedHeaders.Split(';'), StringComparer.Ordinal);
            Assert.Contains(prefix + "date", signedHeaders.Split(';'), StringComparer.Ordinal);
            string canonicalHeaders = string.Concat(signedHeaders.Split(';').Select(name => name + ":" + headers[name] + "\n"));
            string canonical = "HEAD\n/test/object\n\n" + canonicalHeaders + "\n" + signedHeaders + "\n" + headers[prefix + "content-sha256"];
            string canonicalHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
            string toSign = algorithm + "\n" + headers[prefix + "date"] + "\n" + scope + "\n" + canonicalHash;
            byte[] key = Encoding.UTF8.GetBytes("AWS4secret");
            foreach (string component in scope.Split('/'))
                key = HMACSHA256.HashData(key, Encoding.UTF8.GetBytes(component));
            string expected = Convert.ToHexString(HMACSHA256.HashData(key, Encoding.UTF8.GetBytes(toSign))).ToLowerInvariant();
            Assert.Equal("Signature=" + expected, fields[2]);
        }
    }

    [Theory]
    [InlineData("AmazonS3")]
    [InlineData("BackBlazeB2")]
    [InlineData("Wasabi")]
    public void PublicPresigningTracksRegionMutation(string providerName)
    {
        using ServiceProvider provider = CreateSigningProvider(new NullNetworkDriver(), providerName);
        ISimpleClient client = provider.GetRequiredService<ISimpleClient>();
        SimpleS3Config config = provider.GetRequiredService<IOptions<SimpleS3Config>>().Value;
        config.Endpoint = "https://s3.{Region}.example.test";
        const string prefix = "X-Amz-";
        const string algorithm = "AWS4-HMAC-SHA256";
        const string signedHeaders = "host";

        foreach (string region in new[] { "us-east-1", "eu-west-1", "us-east-1" })
        {
            config.RegionCode = region;
            HeadObjectRequest request = new HeadObjectRequest("test", "object");
            Uri url = new Uri(client.SignRequest(request, TimeSpan.FromMinutes(1)));
            Assert.Equal("s3." + region + ".example.test", url.Host);
            Assert.Equal("/test/object", url.AbsolutePath);
            Assert.Equal(signedHeaders, request.QueryParameters[prefix + "SignedHeaders"]);
            Assert.Equal(algorithm, request.QueryParameters[prefix + "Algorithm"]);
            string credential = request.QueryParameters[prefix + "Credential"];
            string scope = credential[(credential.IndexOf('/', StringComparison.Ordinal) + 1)..];
            Assert.EndsWith("/" + region + "/s3/aws4_request", scope, StringComparison.Ordinal);
            string canonicalQuery = string.Join('&', request.QueryParameters
                                                            .Where(pair => pair.Key != prefix + "Signature" && !pair.Key.StartsWith("simples3-header-", StringComparison.Ordinal))
                                                            .Select(pair => (Key: Uri.EscapeDataString(pair.Key), Value: Uri.EscapeDataString(pair.Value)))
                                                            .OrderBy(pair => pair.Key, StringComparer.Ordinal).ThenBy(pair => pair.Value, StringComparer.Ordinal)
                                                            .Select(pair => pair.Key + "=" + pair.Value));
            string canonicalHeaders = "host:" + url.Host + "\n";
            string canonical = "HEAD\n/test/object\n" + canonicalQuery + "\n" + canonicalHeaders + "\n" + signedHeaders + "\nUNSIGNED-PAYLOAD";
            string canonicalHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
            string toSign = algorithm + "\n" + request.QueryParameters[prefix + "Date"] + "\n" + scope + "\n" + canonicalHash;
            byte[] key = Encoding.UTF8.GetBytes("AWS4secret");
            foreach (string component in scope.Split('/'))
                key = HMACSHA256.HashData(key, Encoding.UTF8.GetBytes(component));
            string expected = Convert.ToHexString(HMACSHA256.HashData(key, Encoding.UTF8.GetBytes(toSign))).ToLowerInvariant();
            Assert.Equal(expected, request.QueryParameters[prefix + "Signature"]);
        }
    }

    [Theory]
    [InlineData("AmazonS3", "c7bc1bf0855c3f2f8879ccfb7a0c0948345f90bfe7513a47352255bd6750c8db", "fac88331d6cb105638fec18f5329dcaee9cd6898cd31d33b5c47f0a81c5ef252")]
    public void SigningKeyTracksRegionMutation(string providerName, string initialHex, string changedHex)
    {
        SimpleS3Config config = new SimpleS3Config
        {
            ProviderName = providerName,
            RegionCode = "us-east-1",
            Credentials = new StringAccessKey("key", "secret")
        };
        SigningKeyBuilder builder = new SigningKeyBuilder(Options.Create(config), NullLogger<SigningKeyBuilder>.Instance);
        DateTimeOffset date = new DateTimeOffset(2026, 9, 8, 0, 0, 0, TimeSpan.Zero);

        Assert.Equal(Convert.FromHexString(initialHex), builder.CreateSigningKey(date));
        config.RegionCode = "eu-west-1";
        Assert.Equal(Convert.FromHexString(changedHex), builder.CreateSigningKey(date));
        config.RegionCode = "us-east-1";
        Assert.Equal(Convert.FromHexString(initialHex), builder.CreateSigningKey(date));
    }

    [Fact]
    public void PreSignRequireCredentials()
    {
        ServiceCollection collection = new ServiceCollection();
        collection.Configure<SimpleS3Config>(config =>
        {
            config.Endpoint = "http://something.com";
            config.RegionCode = "us-east-1";
            config.NamingMode = NamingMode.PathStyle;
            config.Credentials = null;
        });

        ICoreBuilder builder = SimpleS3CoreServices.AddSimpleS3Core(collection);
        builder.UseHttpClientFactory();

        using ServiceProvider provider = collection.BuildServiceProvider();
        ISimpleClient client = provider.GetRequiredService<ISimpleClient>();

        Assert.Throws<InvalidOperationException>(() => client.SignRequest(new GetObjectRequest("test", "name"), TimeSpan.FromHours(1)));
    }

    [Fact]
    public void PreSignDoesNotDisposeRequestContent()
    {
        ServiceCollection collection = new ServiceCollection();
        collection.Configure<SimpleS3Config>(config =>
        {
            config.Endpoint = "http://something.com";
            config.RegionCode = "us-east-1";
            config.NamingMode = NamingMode.PathStyle;
            config.Credentials = new StringAccessKey("key", "secret");
        });

        ICoreBuilder builder = SimpleS3CoreServices.AddSimpleS3Core(collection);
        builder.UseHttpClientFactory();

        using ServiceProvider provider = collection.BuildServiceProvider();
        ISimpleClient client = provider.GetRequiredService<ISimpleClient>();
        using MemoryStream content = new MemoryStream([1, 2, 3]);
        PutObjectRequest request = new PutObjectRequest("test", "name", content);

        _ = client.SignRequest(request, TimeSpan.FromHours(1));

        Assert.True(content.CanRead);
    }

    private static ServiceProvider CreateSigningProvider(INetworkDriver? network = null, string? providerName = null)
    {
        ServiceCollection collection = new ServiceCollection();
        collection.Configure<SimpleS3Config>(config =>
        {
            config.ProviderName = providerName ?? string.Empty;
            config.Endpoint = "http://something.com";
            config.RegionCode = "us-east-1";
            config.NamingMode = NamingMode.PathStyle;
            config.Credentials = new StringAccessKey("key", "secret");
        });

        ICoreBuilder builder = SimpleS3CoreServices.AddSimpleS3Core(collection);
        builder.UseHttpClientFactory();
        if (network != null)
            collection.AddSingleton(network);
        return collection.BuildServiceProvider();
    }
}