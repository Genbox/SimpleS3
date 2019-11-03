using System;
using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Tests.LiveTests.Objects
{
    public class GenericTests : LiveTestBase
    {
        public GenericTests(ITestOutputHelper helper) : base(helper)
        {
            Config.EnableStreaming = false;
        }

        [Fact]
        public async Task BasicCRDTest()
        {
            PutObjectResponse putResp = await UploadAsync(nameof(BasicCRDTest)).ConfigureAwait(false);
            Assert.Equal(200, putResp.StatusCode);

            //Test lifecycle expiration (yes, we add 2 days. I don't know why Amazon works like this)
            Assert.Equal(DateTime.UtcNow.AddDays(2).Date, putResp.LifeCycleExpiresOn.Value.UtcDateTime.Date);
            Assert.Equal("AllExpire", putResp.LifeCycleRuleId);

            GetObjectResponse getResp = await AssertAsync(nameof(BasicCRDTest)).ConfigureAwait(false);

            //Test lifecycle expiration
            Assert.Equal(DateTime.UtcNow.AddDays(2).Date, putResp.LifeCycleExpiresOn.Value.UtcDateTime.Date);
            Assert.Equal("AllExpire", getResp.LifeCycleRuleId);

            DeleteObjectResponse deleteResp = await ObjectClient.DeleteObjectAsync(BucketName, nameof(BasicCRDTest)).ConfigureAwait(false);
            Assert.Equal(204, deleteResp.StatusCode);
        }

        [Fact]
        public async Task HeadTest()
        {
            await UploadAsync(nameof(BasicCRDTest)).ConfigureAwait(false);

            HeadObjectResponse gResp = await ObjectClient.HeadObjectAsync(BucketName, nameof(BasicCRDTest)).ConfigureAwait(false);
            Assert.Equal(200, gResp.StatusCode);
        }
    }
}