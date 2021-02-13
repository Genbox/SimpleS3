using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Enums;
using Genbox.SimpleS3.Core.Network.Requests.S3Types;
using Genbox.SimpleS3.Core.Network.Responses.Buckets;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Extensions.AwsS3.Tests.Online.Buckets
{
    public class BucketLifecycleConfigurationTests : AwsTestBase
    {
        public BucketLifecycleConfigurationTests(ITestOutputHelper helper) : base(helper) { }

        [Fact]
        public async Task PutLifecycleConfigurationTest()
        {
            await CreateTempBucketAsync(async x =>
            {
                IList<S3Rule> rules = new List<S3Rule>
                {
                    new S3Rule("Transition logs after 30 days to StandardIa and after 60 days to OneZoneIa", true)
                    {
                        Transitions = new List<S3Transition>
                        {
                            new S3Transition(30, StorageClass.StandardIa),
                            new S3Transition(60, StorageClass.OneZoneIa)
                        },
                        Filter = new S3Filter { Prefix = "logs/" }
                    },
                    new S3Rule("Expire temp folder after 5 days", true)
                    {
                        Expiration = new S3Expiration(5),
                        Filter = new S3Filter { Prefix = "temp/" }
                    },
                    new S3Rule("Expire temp2 folder tomorrow", true)
                    {
                        Expiration = new S3Expiration(DateTimeOffset.UtcNow.AddDays(1)),
                        Filter = new S3Filter { Prefix = "temp2/" }
                    }
                };

                PutBucketLifecycleConfigurationResponse putResp = await BucketClient.PutBucketLifecycleConfigurationAsync(x, rules).ConfigureAwait(false);
                Assert.True(putResp.IsSuccess);

            }).ConfigureAwait(false);
        }
    }
}