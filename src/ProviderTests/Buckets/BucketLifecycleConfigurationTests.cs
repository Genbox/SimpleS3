using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Genbox.ProviderTests.Misc;
using Genbox.SimpleS3.Core.Abstracts;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Genbox.SimpleS3.Utility.Shared;
using Xunit;

namespace Genbox.ProviderTests.Buckets
{
    public class BucketLifecycleConfigurationTests : TestBase
    {
        [Theory]
        [MultipleProviders(S3Provider.AmazonS3)]
        public async Task PutGetLifecycleConfigurationTest(S3Provider provider, string _, ISimpleClient client)
        {
            await CreateTempBucketAsync(provider, client, async tempBucket =>
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

                PutBucketLifecycleConfigurationResponse putResp = await client.PutBucketLifecycleConfigurationAsync(tempBucket, new[] { rule1, rule2, rule3 }).ConfigureAwait(false);
                Assert.True(putResp.IsSuccess);

                GetBucketLifecycleConfigurationResponse getResp = await client.GetBucketLifecycleConfigurationAsync(tempBucket).ConfigureAwait(false);
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
            }).ConfigureAwait(false);
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3)]
        public async Task PutLifecycleConfigurationBucketWideTest(S3Provider provider, string _, ISimpleClient client)
        {
            await CreateTempBucketAsync(provider, client, async tempBucket =>
            {
                S3Rule rule = new S3Rule("Expire the whole bucket after 10 days", true);
                rule.Expiration = new S3Expiration(DateTimeOffset.UtcNow.AddDays(10));
                rule.Filter = new S3Filter(); //Empty filter means the whole bucket is affected

                PutBucketLifecycleConfigurationResponse putResp = await client.PutBucketLifecycleConfigurationAsync(tempBucket, new[] { rule }).ConfigureAwait(false);
                Assert.True(putResp.IsSuccess);

                GetBucketLifecycleConfigurationResponse getResp = await client.GetBucketLifecycleConfigurationAsync(tempBucket);
                Assert.True(getResp.IsSuccess);

                S3Rule rule1A = getResp.Rules[0];
                Assert.Equal(rule.Id, rule1A.Id);
                Assert.Equal(rule.Enabled, rule1A.Enabled);
                Assert.Equal(rule.Filter.Prefix, rule1A.Filter?.Prefix);
                Assert.Equal(rule.Expiration.ExpireAfterDays, rule1A.Expiration?.ExpireAfterDays);

            }).ConfigureAwait(false);
        }

        [Theory]
        [MultipleProviders(S3Provider.AmazonS3)]
        public async Task PutLifecycleConfigurationWithLogicalAndTest(S3Provider provider, string _, ISimpleClient client)
        {
            await CreateTempBucketAsync(provider, client, async tempBucket =>
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

                PutBucketLifecycleConfigurationResponse putResp = await client.PutBucketLifecycleConfigurationAsync(tempBucket, new[] { rule }).ConfigureAwait(false);
                Assert.True(putResp.IsSuccess);

                GetBucketLifecycleConfigurationResponse getResp = await client.GetBucketLifecycleConfigurationAsync(tempBucket).ConfigureAwait(false);
                Assert.True(getResp.IsSuccess);

                S3Rule? rule1 = Assert.Single(getResp.Rules);
                S3AndCondition? conditions1 = rule1.Filter?.AndConditions;

                Assert.NotNull(conditions1);
                Assert.Equal(conditions.Prefix, conditions1!.Prefix);
                Assert.Equal(conditions.Tags, conditions1.Tags);

            }).ConfigureAwait(false);
        }
    }
}