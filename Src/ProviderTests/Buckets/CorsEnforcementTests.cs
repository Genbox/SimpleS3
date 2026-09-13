using System.Net;
using System.Net.Http.Headers;
using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Network.Requests.Objects;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class CorsEnforcementTests : TestBase
{
    private const int MaxPollAttempts = 120;
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(1);

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.BackBlazeB2 | S3Provider.Wasabi)]
    public async Task PutBucketCorsRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, bucket => CheckCorsAsync(provider, bucket, client, "configuration"));

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3 | S3Provider.BackBlazeB2 | S3Provider.Wasabi, "allowed", "wrong-origin", "wrong-method", "delete")]
    public async Task CorsBehavior(S3Provider provider, string _, ISimpleClient client, string feature) => await CreateTempBucketAsync(provider, client, bucket => CheckCorsAsync(provider, bucket, client, feature));

    private static async Task CheckCorsAsync(S3Provider provider, string bucket, ISimpleClient client, string feature)
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        string origin = "https://cors-" + Guid.NewGuid().ToString("N") + ".example";
        S3CorsRule rule = new S3CorsRule { MaxAgeSeconds = 0 };
        rule.AllowedOrigins.Add(origin);
        rule.AllowedMethods.Add("GET");

        string signed = client.SignRequest(new GetObjectRequest(bucket, "cors-probe-" + Guid.NewGuid().ToString("N")), TimeSpan.FromMinutes(1));
        Uri url = new Uri(new Uri(signed).GetLeftPart(UriPartial.Path));
        Assert.Empty(url.Query);
        using HttpClient http = new HttpClient();

        Assert.True((await client.PutBucketCorsAsync(bucket, [rule], token: token)).IsSuccess);

        if (feature == "configuration")
        {
            GetBucketCorsResponse configured = await client.GetBucketCorsAsync(bucket, token: token);
            Assert.Equal(200, configured.StatusCode);
            S3CorsRule returned = Assert.Single(configured.Rules);
            Assert.Equal(origin, Assert.Single(returned.AllowedOrigins));
            Assert.Equal("GET", Assert.Single(returned.AllowedMethods));
            return;
        }

        await WaitForConfiguredRuleAsync(http, url, origin, provider, token);

        if (feature != "delete")
        {
            await AssertConfiguredPreflightAsync(http, url, origin, feature, token);
            return;
        }

        Assert.True((await client.DeleteBucketCorsAsync(bucket, token: token)).IsSuccess);
        GetBucketCorsResponse readback = await client.GetBucketCorsAsync(bucket, token: token);
        Assert.True(readback.StatusCode == 404 || (readback.IsSuccess && readback.Rules.Count == 0));
        await WaitForProviderDefaultAsync(http, url, origin, provider, token);
    }

    private static async Task WaitForConfiguredRuleAsync(HttpClient http, Uri url, string origin, S3Provider provider, CancellationToken token)
    {
        int consecutiveMatches = 0;

        for (int attempt = 0; attempt < MaxPollAttempts; attempt++)
        {
            using HttpResponseMessage response = await SendPreflightAsync(http, ProbeUrl(url, "ready", attempt), origin, "GET", token);

            // The Wasabi wildcard default must not count as the newly installed restricted rule.
            bool matchesRule = AllowsGet(response, origin) && response.Headers.GetValues("Access-Control-Allow-Origin").Contains(origin, StringComparer.Ordinal);
            consecutiveMatches = matchesRule ? consecutiveMatches + 1 : 0;
            bool ready = consecutiveMatches >= 3;
            if (ready || attempt == MaxPollAttempts - 1)
                TestContext.Current.TestOutputHelper?.WriteLine($"{provider}: configuration ready={ready}, observations={attempt + 1}");
            if (ready)
                return;
            await Task.Delay(PollInterval, token);
        }

        Assert.True(consecutiveMatches >= 3, "The configured preflight permission did not become effective within the polling window.");
    }

    private static async Task AssertConfiguredPreflightAsync(HttpClient http, Uri url, string origin, string feature, CancellationToken token)
    {
        using HttpResponseMessage allowed = await SendPreflightAsync(http, ProbeUrl(url, feature, 0), feature == "wrong-origin" ? "https://not-allowed.example" : origin, feature == "wrong-method" ? "POST" : "GET", token);

        if (feature != "allowed")
        {
            Assert.Equal(HttpStatusCode.Forbidden, allowed.StatusCode);
            Assert.False(allowed.Headers.Contains("Access-Control-Allow-Origin"));
            return;
        }

        Assert.Equal(HttpStatusCode.OK, allowed.StatusCode);
        Assert.True(allowed.Headers.TryGetValues("Access-Control-Allow-Origin", out IEnumerable<string>? origins));
        Assert.Contains(origin, origins);
        Assert.True(allowed.Headers.TryGetValues("Access-Control-Allow-Methods", out IEnumerable<string>? methods));
        Assert.Contains("GET", methods.SelectMany(value => value.Split(',')).Select(value => value.Trim()));
    }

    private static async Task WaitForProviderDefaultAsync(HttpClient http, Uri url, string origin, S3Provider provider, CancellationToken token)
    {
        bool expectedDefault = provider == S3Provider.Wasabi;
        bool defaultRestored = false;

        for (int attempt = 0; attempt < MaxPollAttempts; attempt++)
        {
            // Use a fresh resource to distinguish configuration state from stale responses for the original URL.
            using HttpResponseMessage removed = await SendPreflightAsync(http, ProbeUrl(url, "after-delete", attempt), origin, "GET", token, true);
            bool stillAllowed = AllowsGet(removed, origin);
            defaultRestored = expectedDefault
                ? removed.StatusCode == HttpStatusCode.NoContent && stillAllowed && removed.Headers.GetValues("Access-Control-Allow-Origin").Contains("*", StringComparer.Ordinal)
                : removed.StatusCode == HttpStatusCode.Forbidden && !removed.Headers.Contains("Access-Control-Allow-Origin");
            if (attempt is 0 || attempt == MaxPollAttempts - 1 || defaultRestored)
                TestContext.Current.TestOutputHelper?.WriteLine($"{provider}: after deletion HTTP={(int)removed.StatusCode}, preflight-authorized={stillAllowed}");
            if (defaultRestored)
                return;
            await Task.Delay(PollInterval, token);
        }

        Assert.True(defaultRestored, "The provider default did not become effective within the polling window.");
    }

    private static async Task<HttpResponseMessage> SendPreflightAsync(HttpClient http, Uri url, string origin, string method, CancellationToken token, bool noCache = false)
    {
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Options, url);
        request.Headers.Add("Origin", origin);
        request.Headers.Add("Access-Control-Request-Method", method);
        if (noCache)
            request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
        return await http.SendAsync(request, token);
    }

    private static Uri ProbeUrl(Uri url, string phase, int attempt) => new Uri(url.AbsoluteUri + "-" + phase + "-" + attempt);

    private static bool AllowsGet(HttpResponseMessage response, string origin) =>
        response.IsSuccessStatusCode &&
        response.Headers.TryGetValues("Access-Control-Allow-Origin", out IEnumerable<string>? origins) &&
        origins.Any(value => value == origin || value == "*") &&
        response.Headers.TryGetValues("Access-Control-Allow-Methods", out IEnumerable<string>? methods) &&
        methods.SelectMany(value => value.Split(',')).Any(value => value.Trim() == "GET");
}