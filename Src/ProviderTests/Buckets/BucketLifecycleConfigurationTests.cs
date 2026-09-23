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
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task DatedGlacierTransitionConfigurationPersists(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        DateTimeOffset expected = new DateTimeOffset(DateTime.UtcNow.Date.AddDays(30), TimeSpan.Zero);
        DateTimeOffset supplied = expected.AddHours(23).ToOffset(TimeSpan.FromHours(2));
        S3Rule rule = new S3Rule("audit-dated-transition", true)
        {
            Filter = new S3Filter { Prefix = "audit/" }
        };
        rule.Transitions.Add(new S3Transition(supplied, StorageClass.Glacier));
        rule.Expiration = new S3Expiration(supplied.AddDays(365));
        Assert.Equal(200, (await client.PutBucketLifecycleConfigurationAsync(bucket, [rule], token: token)).StatusCode);
        GetBucketLifecycleConfigurationResponse get = await ReadExpectedLifecycleConfigurationAsync(client, bucket,
            response => response.StatusCode == 200 && response.Rules.Count == 1 && response.Rules[0].Id == rule.Id, token);
        Assert.Equal(200, get.StatusCode);
        S3Rule returned = Assert.Single(get.Rules);
        Assert.Equal(rule.Id, returned.Id);
        Assert.True(returned.Enabled);
        Assert.Equal("audit/", returned.Filter?.Prefix);
        S3Transition transition = Assert.Single(returned.Transitions);
        Assert.Equal(expected, transition.TransitionOnDate);
        Assert.Null(transition.TransitionAfterDays);
        Assert.Equal(StorageClass.Glacier, transition.StorageClass);
        Assert.Equal(supplied, rule.Transitions[0].TransitionOnDate);
        Assert.NotNull(returned.Expiration);
        Assert.Equal(expected.AddDays(365), returned.Expiration.ExpireOnDate);
        Assert.Null(returned.Expiration.ExpireAfterDays);
        Assert.Null(returned.Expiration.ExpireObjectDeleteMarker);
        Assert.Equal(supplied.AddDays(365), rule.Expiration.ExpireOnDate);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task NoncurrentTransitionRetentionBoundariesPreserveConfiguration(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        Assert.Equal(200, (await client.PutBucketVersioningAsync(bucket, true, token: token)).StatusCode);
        S3Rule rule = new S3Rule("audit-noncurrent-transition", true)
        {
            Filter = new S3Filter { Prefix = "audit/" }
        };
        S3NonCurrentVersionTransition transition = new S3NonCurrentVersionTransition(1, StorageClass.Glacier);
        rule.NonCurrentVersionTransitions.Add(transition);

        foreach (int count in new[] { 1, 100, 0, 101 })
        {
            transition.NewerNonCurrentVersions = count;
            PutBucketLifecycleConfigurationResponse put = await client.PutBucketLifecycleConfigurationAsync(bucket, [rule], token: token);

            // AWS currently accepts 101 despite the documented maximum of 100.
            Assert.Equal(count == 0 ? 400 : 200, put.StatusCode);
            int expectedCount = count == 0 ? 100 : count;
            GetBucketLifecycleConfigurationResponse get = await ReadExpectedLifecycleConfigurationAsync(client, bucket,
                response => response.StatusCode == 200 && response.Rules.Count == 1 && response.Rules[0].NonCurrentVersionTransitions.Count == 1 &&
                            response.Rules[0].NonCurrentVersionTransitions[0].NewerNonCurrentVersions == expectedCount, token);

            S3Rule returned = Assert.Single(get.Rules);
            Assert.Equal(rule.Id, returned.Id);
            Assert.True(returned.Enabled);
            Assert.Equal("audit/", returned.Filter?.Prefix);
            S3NonCurrentVersionTransition actual = Assert.Single(returned.NonCurrentVersionTransitions);
            Assert.Equal(expectedCount, actual.NewerNonCurrentVersions);
            Assert.Equal(1, actual.NonCurrentDays);
            Assert.Equal(StorageClass.Glacier, actual.StorageClass);
        }

        rule.Filter = null;
        transition.NewerNonCurrentVersions = 1;
        PutBucketLifecycleConfigurationResponse missingFilter = await client.PutBucketLifecycleConfigurationAsync(bucket, [rule], token: token);
        Assert.Equal(400, missingFilter.StatusCode);
        Assert.Equal(ErrorCode.MalformedXML, missingFilter.Error?.Code);
        GetBucketLifecycleConfigurationResponse preserved = await ReadExpectedLifecycleConfigurationAsync(client, bucket,
            response => response.StatusCode == 200 && response.Rules.Count == 1 && response.Rules[0].NonCurrentVersionTransitions.Count == 1 &&
                        response.Rules[0].NonCurrentVersionTransitions[0].NewerNonCurrentVersions == 101, token);

        S3Rule preservedRule = Assert.Single(preserved.Rules);
        Assert.Equal(rule.Id, preservedRule.Id);
        Assert.True(preservedRule.Enabled);
        Assert.Equal("audit/", preservedRule.Filter?.Prefix);
        S3NonCurrentVersionTransition preservedTransition = Assert.Single(preservedRule.NonCurrentVersionTransitions);
        Assert.Equal(101, preservedTransition.NewerNonCurrentVersions);
        Assert.Equal(1, preservedTransition.NonCurrentDays);
        Assert.Equal(StorageClass.Glacier, preservedTransition.StorageClass);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task ZeroDayGlacierTransitionConfigurationPersists(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        S3Rule rule = new S3Rule("audit-zero-day-transition", true)
        {
            Filter = new S3Filter { Prefix = "audit/" }
        };
        rule.Transitions.Add(new S3Transition(0, StorageClass.Glacier));
        Assert.Equal(200, (await client.PutBucketLifecycleConfigurationAsync(bucket, [rule], token: token)).StatusCode);
        GetBucketLifecycleConfigurationResponse response = await ReadExpectedLifecycleConfigurationAsync(client, bucket,
            current => current.StatusCode == 200 && current.Rules.Count == 1 && current.Rules[0].Id == rule.Id, token);
        Assert.Equal(200, response.StatusCode);
        S3Rule returned = Assert.Single(response.Rules);
        Assert.Equal(rule.Id, returned.Id);
        Assert.True(returned.Enabled);
        Assert.Equal(rule.Filter.Prefix, returned.Filter?.Prefix);
        S3Transition transition = Assert.Single(returned.Transitions);
        Assert.Equal(0, transition.TransitionAfterDays);
        Assert.Equal(StorageClass.Glacier, transition.StorageClass);
        Assert.Null(transition.TransitionOnDate);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3 | S3Provider.Wasabi)]
    public async Task DeleteConfiguredLifecycle(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async bucket =>
    {
        CancellationToken token = TestContext.Current.CancellationToken;
        S3Rule rule = new S3Rule("audit-expiration", true)
        {
            Filter = new S3Filter { Prefix = "audit-expiration/" },
            Expiration = new S3Expiration(30)
        };
        Assert.True((await client.PutBucketLifecycleConfigurationAsync(bucket, [rule], token: token)).IsSuccess);
        GetBucketLifecycleConfigurationResponse configured = await ReadExpectedLifecycleConfigurationAsync(client, bucket,
            response => response.StatusCode == 200 && response.Rules.Count == 1 && response.Rules[0].Id == rule.Id, token);
        Assert.True(configured.IsSuccess);
        S3Rule returnedRule = Assert.Single(configured.Rules);
        Assert.Equal(rule.Id, returnedRule.Id);
        Assert.True(returnedRule.Enabled);
        Assert.Equal(rule.Filter.Prefix, returnedRule.Filter?.Prefix);
        Assert.Equal(30, returnedRule.Expiration?.ExpireAfterDays);

        for (int attempt = 0; attempt < 2; attempt++)
        {
            Assert.Equal(204, (await client.DeleteBucketLifecycleAsync(bucket, token: token)).StatusCode);
            GetBucketLifecycleConfigurationResponse missing = await ReadExpectedLifecycleConfigurationAsync(client, bucket,
                response => response.StatusCode == 404, token);

            Assert.Equal(404, missing.StatusCode);
            Assert.Equal(ErrorCode.NoSuchLifecycleConfiguration, missing.Error?.Code);
        }
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task PutGetLifecycleConfigurationTest(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        S3Rule rule1 = new S3Rule("Transition logs after 30 days to StandardIa and after 60 days to OneZoneIa", true);
        rule1.Transitions.Add(new S3Transition(30, StorageClass.StandardIa));
        rule1.Transitions.Add(new S3Transition(60, StorageClass.OneZoneIa));
        rule1.Filter = new S3Filter { Prefix = "logs/" };

        S3Rule rule2 = new S3Rule("Expire temp folder after 5 days", true);
        rule2.Expiration = new S3Expiration(5);
        rule2.Filter = new S3Filter { Prefix = "temp/" };

        S3Rule rule3 = new S3Rule("Expire things tagged with temp tomorrow", false); //disabled
        rule3.Expiration = new S3Expiration(DateTimeOffset.UtcNow.AddDays(1));
        rule3.Filter = new S3Filter { Tag = new KeyValuePair<string, string>("type", "temp") };

        PutBucketLifecycleConfigurationResponse putResp = await client.PutBucketLifecycleConfigurationAsync(tempBucket, [rule1, rule2, rule3], token: TestContext.Current.CancellationToken);
        Assert.True(putResp.IsSuccess);

        GetBucketLifecycleConfigurationResponse getResp = await ReadExpectedLifecycleConfigurationAsync(client, tempBucket,
            response => response.StatusCode == 200 && response.Rules.Count == 3, TestContext.Current.CancellationToken);
        Assert.True(getResp.IsSuccess);

        S3Rule rule1A = getResp.Rules[0];
        Assert.Equal(rule1.Id, rule1A.Id);
        Assert.Equal(rule1.Enabled, rule1A.Enabled);
        Assert.Equal(rule1.Filter.Prefix, rule1A.Filter?.Prefix);
        Assert.Equal(rule1.Transitions[0].StorageClass, rule1A.Transitions[0].StorageClass);
        Assert.Equal(rule1.Transitions[0].TransitionAfterDays, rule1A.Transitions[0].TransitionAfterDays);
        Assert.Equal(rule1.Transitions[1].StorageClass, rule1A.Transitions[1].StorageClass);
        Assert.Equal(rule1.Transitions[1].TransitionAfterDays, rule1A.Transitions[1].TransitionAfterDays);

        S3Rule rule2A = getResp.Rules[1];
        Assert.Equal(rule2.Id, rule2A.Id);
        Assert.Equal(rule2.Enabled, rule2A.Enabled);
        Assert.Equal(rule2.Filter.Prefix, rule2A.Filter?.Prefix);
        Assert.Equal(rule2.Expiration.ExpireAfterDays, rule2A.Expiration?.ExpireAfterDays);

        S3Rule rule3A = getResp.Rules[2];
        Assert.Equal(rule3.Id, rule3A.Id);
        Assert.Equal(rule3.Enabled, rule3A.Enabled);
        Assert.Equal(rule3.Filter.Tag, rule3A.Filter?.Tag);
        Assert.Equal(rule3.Expiration.ExpireOnDate?.Date, rule3A.Expiration?.ExpireOnDate?.Date); //Amazon round the date to the day instead

        DeleteBucketLifecycleResponse deleteResp = await client.DeleteBucketLifecycleAsync(tempBucket, token: TestContext.Current.CancellationToken);
        Assert.Equal(204, deleteResp.StatusCode);

        GetBucketLifecycleConfigurationResponse missingResp = await ReadExpectedLifecycleConfigurationAsync(client, tempBucket,
            response => response.StatusCode == 404, TestContext.Current.CancellationToken);
        Assert.Equal(404, missingResp.StatusCode);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task PutLifecycleConfigurationBucketWideTest(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        S3Rule rule = new S3Rule("Expire the whole bucket after 10 days", true);
        rule.Expiration = new S3Expiration(DateTimeOffset.UtcNow.AddDays(10));
        rule.Filter = new S3Filter(); //Empty filter means the whole bucket is affected

        PutBucketLifecycleConfigurationResponse putResp = await client.PutBucketLifecycleConfigurationAsync(tempBucket, [rule], token: TestContext.Current.CancellationToken);
        Assert.True(putResp.IsSuccess);

        GetBucketLifecycleConfigurationResponse getResp = await ReadExpectedLifecycleConfigurationAsync(client, tempBucket,
            response => response.StatusCode == 200 && response.Rules.Count == 1 && response.Rules[0].Id == rule.Id, TestContext.Current.CancellationToken);
        Assert.True(getResp.IsSuccess);

        S3Rule rule1A = getResp.Rules[0];
        Assert.Equal(rule.Id, rule1A.Id);
        Assert.Equal(rule.Enabled, rule1A.Enabled);
        Assert.Equal(rule.Filter.Prefix, rule1A.Filter?.Prefix);
        Assert.Equal(rule.Expiration.ExpireAfterDays, rule1A.Expiration?.ExpireAfterDays);
    });

    [Theory]
    [MultipleProviders(S3Provider.AmazonS3)]
    public async Task PutLifecycleConfigurationWithLogicalAndTest(S3Provider provider, string _, ISimpleClient client) => await CreateTempBucketAsync(provider, client, async tempBucket =>
    {
        S3AndCondition conditions = new S3AndCondition();
        conditions.Prefix = "temp/";
        conditions.Tags.Add(new KeyValuePair<string, string>("Type1", "Temp1"));
        conditions.Tags.Add(new KeyValuePair<string, string>("Type2", "Temp2"));

        S3Filter filter = new S3Filter();
        filter.AndConditions = conditions;

        S3Rule rule = new S3Rule("Test logical and", true);
        rule.Filter = filter;
        rule.Expiration = new S3Expiration(DateTimeOffset.UtcNow.AddDays(1));

        PutBucketLifecycleConfigurationResponse putResp = await client.PutBucketLifecycleConfigurationAsync(tempBucket, [rule], token: TestContext.Current.CancellationToken);
        Assert.True(putResp.IsSuccess);

        GetBucketLifecycleConfigurationResponse getResp = await ReadExpectedLifecycleConfigurationAsync(client, tempBucket,
            response => response.StatusCode == 200 && response.Rules.Count == 1 && response.Rules[0].Id == rule.Id, TestContext.Current.CancellationToken);
        Assert.True(getResp.IsSuccess);

        S3Rule rule1 = Assert.Single(getResp.Rules);
        S3AndCondition? conditions1 = rule1.Filter?.AndConditions;

        Assert.NotNull(conditions1);
        Assert.Equal(conditions.Prefix, conditions1.Prefix);
        Assert.Equal(conditions.Tags, conditions1.Tags);
    });
}