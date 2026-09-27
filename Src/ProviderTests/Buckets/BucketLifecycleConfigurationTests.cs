using Genbox.ProviderTests.Code;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Abstracts.Enums;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;

namespace Genbox.ProviderTests.Buckets;

public class BucketLifecycleConfigurationTests : TestBase
{
    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.Wasabi)]
    public async Task PutBucketLifecycleConfigurationRoundTrip(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        S3Rule rule = new S3Rule("expiration", true) { Filter = new S3Filter(), Expiration = new S3Expiration(30) };
        S3Rule returned = await PutAndReadAsync(client, bucket, rule);
        Assert.Equal(30, returned.Expiration?.ExpireAfterDays);
        Assert.Equal(204, (await client.DeleteBucketLifecycleAsync(bucket, token: TestContext.Current.CancellationToken)).StatusCode);
        GetBucketLifecycleConfigurationResponse missing = await ReadExpectedLifecycleConfigurationAsync(client, bucket, response => response.StatusCode == 404, TestContext.Current.CancellationToken);
        Assert.Equal(404, missing.StatusCode);
        Assert.Equal(ErrorCode.NoSuchLifecycleConfiguration, missing.Error?.Code);
    });

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, 0, 30)]
    public async Task TransitionDays(S3Provider provider, string _, ISimpleClient client, int days) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        S3Rule rule = new S3Rule("transition", true) { Filter = new S3Filter() };
        rule.Transitions.Add(new S3Transition(days, StorageClass.Glacier));
        S3Transition transition = Assert.Single((await PutAndReadAsync(client, bucket, rule)).Transitions);
        Assert.Equal(days, transition.TransitionAfterDays);
        Assert.Equal(StorageClass.Glacier, transition.StorageClass);
        Assert.Null(transition.TransitionOnDate);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task TransitionDateNormalizedToUtcDay(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        DateTimeOffset expected = new DateTimeOffset(DateTime.UtcNow.Date.AddDays(30), TimeSpan.Zero);
        DateTimeOffset supplied = expected.AddHours(23).ToOffset(TimeSpan.FromHours(2));
        S3Rule rule = new S3Rule("dated-transition", true) { Filter = new S3Filter() };
        rule.Transitions.Add(new S3Transition(supplied, StorageClass.Glacier));
        S3Transition returned = Assert.Single((await PutAndReadAsync(client, bucket, rule)).Transitions);
        Assert.Equal(expected, returned.TransitionOnDate);
        Assert.Null(returned.TransitionAfterDays);
        Assert.Equal(StorageClass.Glacier, returned.StorageClass);
        Assert.Equal(supplied, rule.Transitions[0].TransitionOnDate);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task BucketWideExpirationDate(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        DateTimeOffset supplied = new DateTimeOffset(DateTime.UtcNow.Date.AddDays(10), TimeSpan.Zero).AddHours(23);
        S3Rule rule = new S3Rule("dated-expiration", true) { Filter = new S3Filter(), Expiration = new S3Expiration(supplied) };
        S3Rule returned = await PutAndReadAsync(client, bucket, rule);
        Assert.Equal(supplied.UtcDateTime.Date, returned.Expiration?.ExpireOnDate?.UtcDateTime.Date);
        Assert.Null(returned.Expiration?.ExpireAfterDays);
        Assert.Equal(supplied, rule.Expiration.ExpireOnDate);
    });

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, "prefix", "tag", "and")]
    public async Task LifecycleFilter(S3Provider provider, string _, ISimpleClient client, string filter) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        S3Filter configured = new S3Filter();
        KeyValuePair<string, string> tag = new KeyValuePair<string, string>("type", "temp");
        if (filter == "prefix") configured.Prefix = "temp/";
        if (filter == "tag") configured.Tag = tag;

        if (filter == "and")
        {
            configured.AndConditions = new S3AndCondition { Prefix = "temp/" };
            configured.AndConditions.Tags.Add(tag);
            configured.AndConditions.Tags.Add(new KeyValuePair<string, string>("owner", "tests"));
        }

        S3Rule rule = new S3Rule("filtered", true) { Filter = configured, Expiration = new S3Expiration(5) };
        S3Rule returned = await PutAndReadAsync(client, bucket, rule);
        Assert.NotNull(returned.Filter);
        if (filter == "prefix") Assert.Equal(configured.Prefix, returned.Filter.Prefix);
        if (filter == "tag") Assert.Equal(tag, returned.Filter.Tag);

        if (filter == "and")
        {
            Assert.NotNull(returned.Filter.AndConditions);
            Assert.Equal(configured.AndConditions!.Prefix, returned.Filter.AndConditions.Prefix);
            Assert.Equal(configured.AndConditions.Tags, returned.Filter.AndConditions.Tags);
        }
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task DisabledRule(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        S3Rule rule = new S3Rule("disabled", false) { Filter = new S3Filter(), Expiration = new S3Expiration(5) };
        Assert.False((await PutAndReadAsync(client, bucket, rule)).Enabled);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task MultipleTransitions(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        S3Rule rule = new S3Rule("transitions", true) { Filter = new S3Filter() };
        rule.Transitions.Add(new S3Transition(30, StorageClass.StandardIa));
        rule.Transitions.Add(new S3Transition(60, StorageClass.OneZoneIa));
        S3Rule returned = await PutAndReadAsync(client, bucket, rule);
        Assert.Equal(2, returned.Transitions.Count);
        Assert.Contains(returned.Transitions, transition => transition.TransitionAfterDays == 30 && transition.StorageClass == StorageClass.StandardIa);
        Assert.Contains(returned.Transitions, transition => transition.TransitionAfterDays == 60 && transition.StorageClass == StorageClass.OneZoneIa);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task MultipleRules(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        S3Rule[] rules = [new S3Rule("first", true) { Filter = new S3Filter { Prefix = "first/" }, Expiration = new S3Expiration(5) }, new S3Rule("second", true) { Filter = new S3Filter { Prefix = "second/" }, Expiration = new S3Expiration(10) }];
        Assert.True((await client.PutBucketLifecycleConfigurationAsync(bucket, rules, token: TestContext.Current.CancellationToken)).IsSuccess);
        GetBucketLifecycleConfigurationResponse response = await ReadExpectedLifecycleConfigurationAsync(client, bucket, current => current.IsSuccess && current.Rules.Count == 2, TestContext.Current.CancellationToken);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal(2, response.Rules.Count);

        foreach (S3Rule rule in rules)
        {
            S3Rule returned = Assert.Single(response.Rules, item => item.Id == rule.Id);
            Assert.Equal(rule.Filter!.Prefix, returned.Filter?.Prefix);
            Assert.Equal(rule.Expiration!.ExpireAfterDays, returned.Expiration?.ExpireAfterDays);
        }
    });

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, 1, 100, 101)]
    public async Task NoncurrentRetentionCount(S3Provider provider, string _, ISimpleClient client, int count) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        Assert.Equal(200, (await client.PutBucketVersioningAsync(bucket, true, token: TestContext.Current.CancellationToken)).StatusCode);
        S3Rule rule = new S3Rule("noncurrent", true) { Filter = new S3Filter { Prefix = "audit/" } };
        rule.NonCurrentVersionTransitions.Add(new S3NonCurrentVersionTransition(1, StorageClass.Glacier) { NewerNonCurrentVersions = count });

        // AWS accepts 101 despite the documented maximum of 100.
        S3NonCurrentVersionTransition returned = Assert.Single((await PutAndReadAsync(client, bucket, rule)).NonCurrentVersionTransitions);
        Assert.Equal(count, returned.NewerNonCurrentVersions);
        Assert.Equal(1, returned.NonCurrentDays);
        Assert.Equal(StorageClass.Glacier, returned.StorageClass);
    });

    [Theory]
    [MultipleProvidersWithData(S3Provider.AmazonS3, "zero", "missing-filter")]
    public async Task InvalidNoncurrentRetentionPreservesConfiguration(S3Provider provider, string _, ISimpleClient client, string invalid) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        Assert.Equal(200, (await client.PutBucketVersioningAsync(bucket, true, token: TestContext.Current.CancellationToken)).StatusCode);
        S3Rule rule = new S3Rule("noncurrent", true) { Filter = new S3Filter { Prefix = "audit/" } };
        S3NonCurrentVersionTransition transition = new S3NonCurrentVersionTransition(1, StorageClass.Glacier) { NewerNonCurrentVersions = 1 };
        rule.NonCurrentVersionTransitions.Add(transition);
        await PutAndReadAsync(client, bucket, rule);
        if (invalid == "zero") transition.NewerNonCurrentVersions = 0;
        else rule.Filter = null;
        PutBucketLifecycleConfigurationResponse rejected = await client.PutBucketLifecycleConfigurationAsync(bucket, [rule], token: TestContext.Current.CancellationToken);
        Assert.Equal(400, rejected.StatusCode);
        if (invalid == "missing-filter") Assert.Equal(ErrorCode.MalformedXML, rejected.Error?.Code);
        GetBucketLifecycleConfigurationResponse response = await client.GetBucketLifecycleConfigurationAsync(bucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(200, response.StatusCode);
        S3Rule preserved = Assert.Single(response.Rules);
        Assert.Equal("audit/", preserved.Filter?.Prefix);
        Assert.Equal(1, Assert.Single(preserved.NonCurrentVersionTransitions).NewerNonCurrentVersions);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.Wasabi)]
    public async Task DeleteMissingConfiguration(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        Assert.Equal(204, (await client.DeleteBucketLifecycleAsync(bucket, token: TestContext.Current.CancellationToken)).StatusCode);
        Assert.Equal(204, (await client.DeleteBucketLifecycleAsync(bucket, token: TestContext.Current.CancellationToken)).StatusCode);
    });

    private static async Task<S3Rule> PutAndReadAsync(ISimpleClient client, string bucket, S3Rule rule)
    {
        Assert.True((await client.PutBucketLifecycleConfigurationAsync(bucket, [rule], token: TestContext.Current.CancellationToken)).IsSuccess);
        GetBucketLifecycleConfigurationResponse response = await ReadExpectedLifecycleConfigurationAsync(client, bucket, current => current.IsSuccess && current.Rules.Count == 1 && current.Rules[0].Id == rule.Id, TestContext.Current.CancellationToken);
        Assert.Equal(200, response.StatusCode);
        S3Rule returned = Assert.Single(response.Rules);
        Assert.Equal(rule.Id, returned.Id);
        Assert.Equal(rule.Enabled, returned.Enabled);
        return returned;
    }
}