using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.OnlineTests.Objects
{
    public class GetObjectTests : OnlineTestBase
    {
        public GetObjectTests(ITestOutputHelper helper) : base(helper)
        {
        }

        [Fact]
        public async Task GetObjectContentRange()
        {
            await UploadAsync(nameof(GetObjectContentRange)).ConfigureAwait(false);

            GetObjectResponse getResp = await ObjectClient.GetObjectAsync(BucketName, nameof(GetObjectContentRange), req => req.Range.Add(0, 2)).ConfigureAwait(false);

            Assert.Equal(206, getResp.StatusCode);
            Assert.Equal(3, getResp.ContentLength);
            Assert.Equal("bytes", getResp.AcceptRanges);
            Assert.Equal("bytes 0-2/4", getResp.ContentRange);
            Assert.Equal("tes", await getResp.Content.AsStringAsync().ConfigureAwait(false));
        }

        [Fact]
        public async Task GetObjectLifecycle()
        {
            PutObjectResponse putResp = await UploadAsync(nameof(GetObjectLifecycle)).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            //Test lifecycle expiration (yes, we add 2 days. I don't know why Amazon works like this)
            Assert.Equal(DateTime.UtcNow.AddDays(2).Date, putResp.LifeCycleExpiresOn.Value.UtcDateTime.Date);
            Assert.Equal("ExpireAll", putResp.LifeCycleRuleId);

            GetObjectResponse getResp = await AssertAsync(nameof(GetObjectLifecycle)).ConfigureAwait(false);

            //Test lifecycle expiration
            Assert.Equal(DateTime.UtcNow.AddDays(2).Date, getResp.LifeCycleExpiresOn.Value.UtcDateTime.Date);
            Assert.Equal("ExpireAll", getResp.LifeCycleRuleId);
        }
    }
}