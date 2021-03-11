using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Extensions.AmazonS3.Tests.Online.Buckets
{
    public class BucketLifecycleConfigurationTests : AwsTestBase
    {
        public BucketLifecycleConfigurationTests(ITestOutputHelper helper) : base(helper) { }

        [Fact]
        public async Task PutGetLifecycleConfigurationTest()
        {
            await CreateTempBucketAsync(async x =>
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

                PutBucketLifecycleConfigurationResponse putResp = await BucketClient.PutBucketLifecycleConfigurationAsync(x, new[] { rule1, rule2, rule3 }).ConfigureAwait(false);
                Assert.True(putResp.IsSuccess);

                GetBucketLifecycleConfigurationResponse getResp = await BucketClient.GetBucketLifecycleConfigurationAsync(x).ConfigureAwait(false);
                Assert.True(getResp.IsSuccess);

                S3Rule rule1a = getResp.Rules[0];
                Assert.Equal(rule1.Id, rule1a.Id);
                Assert.Equal(rule1.Enabled, rule1a.Enabled);
                Assert.Equal(rule1.Filter.Prefix, rule1a.Filter?.Prefix);
                Assert.Equal(rule1.Transitions[0].StorageClass, rule1a.Transitions[0].StorageClass);
                Assert.Equal(rule1.Transitions[0].TransitionAfterDays, rule1a.Transitions[0].TransitionAfterDays);
                Assert.Equal(rule1.Transitions[1].StorageClass, rule1a.Transitions[1].StorageClass);
                Assert.Equal(rule1.Transitions[1].TransitionAfterDays, rule1a.Transitions[1].TransitionAfterDays);

                S3Rule rule2a = getResp.Rules[1];
                Assert.Equal(rule2.Id, rule2a.Id);
                Assert.Equal(rule2.Enabled, rule2a.Enabled);
                Assert.Equal(rule2.Filter.Prefix, rule2a.Filter?.Prefix);
                Assert.Equal(rule2.Expiration.ExpireAfterDays, rule2a.Expiration?.ExpireAfterDays);

                S3Rule rule3a = getResp.Rules[2];
                Assert.Equal(rule3.Id, rule3a.Id);
                Assert.Equal(rule3.Enabled, rule3a.Enabled);
                Assert.Equal(rule3.Filter.Tag, rule3a.Filter?.Tag);
                Assert.Equal(rule3.Expiration.ExpireOnDate?.Date, rule3a.Expiration?.ExpireOnDate?.Date); //Amazon round the date to the day instead
            }).ConfigureAwait(false);
        }

        [Fact]
        public async Task PutLifecycleConfigurationBucketWideTest()
        {
            await CreateTempBucketAsync(async x =>
            {
                S3Rule rule = new S3Rule("Expire the whole bucket after 10 days", true);
                rule.Expiration = new S3Expiration(DateTimeOffset.UtcNow.AddDays(10));
                rule.Filter = new S3Filter(); //Empty filter means the whole bucket is affected

                PutBucketLifecycleConfigurationResponse putResp = await BucketClient.PutBucketLifecycleConfigurationAsync(x, new[] { rule }).ConfigureAwait(false);
                Assert.True(putResp.IsSuccess);

                GetBucketLifecycleConfigurationResponse getResp = await BucketClient.GetBucketLifecycleConfigurationAsync(x);
                Assert.True(getResp.IsSuccess);

                S3Rule rule1a = getResp.Rules[0];
                Assert.Equal(rule.Id, rule1a.Id);
                Assert.Equal(rule.Enabled, rule1a.Enabled);
                Assert.Equal(rule.Filter.Prefix, rule1a.Filter?.Prefix);
                Assert.Equal(rule.Expiration.ExpireAfterDays, rule1a.Expiration?.ExpireAfterDays);

            }).ConfigureAwait(false);
        }

        [Fact(Skip = "This does not currently work. Seems to match docs, but AWS gives an error")]
        public async Task PutLifecycleConfigurationWithLogicalAndTest()
        {
            await CreateTempBucketAsync(async x =>
            {
                S3Filter filter1 = new S3Filter();
                filter1.Tag = new KeyValuePair<string, string>("type", "temp");

                S3Filter filter2 = new S3Filter();
                filter2.Prefix = "Temp/";

                S3Rule rule = new S3Rule("Test logical and", true);
                rule.Filter = new S3Filter { Conditions = { filter1, filter2 } };
                rule.Expiration = new S3Expiration(DateTimeOffset.UtcNow.AddDays(1));

                PutBucketLifecycleConfigurationResponse putResp = await BucketClient.PutBucketLifecycleConfigurationAsync(x, new[] { rule }).ConfigureAwait(false);
                Assert.True(putResp.IsSuccess);

            }).ConfigureAwait(false);
        }
    }
}