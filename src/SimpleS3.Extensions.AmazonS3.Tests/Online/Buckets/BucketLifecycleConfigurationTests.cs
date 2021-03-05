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
        public async Task PutLifecycleConfigurationTest()
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