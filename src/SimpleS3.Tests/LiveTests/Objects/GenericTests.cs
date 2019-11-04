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
            Assert.Equal(DateTime.UtcNow.AddDays(2).Date, getResp.LifeCycleExpiresOn.Value.UtcDateTime.Date);
            Assert.Equal("AllExpire", getResp.LifeCycleRuleId);

            HeadObjectResponse headResp = await ObjectClient.HeadObjectAsync(BucketName, nameof(BasicCRDTest)).ConfigureAwait(false);

            //Expiration should work on head too
            Assert.Equal(DateTime.UtcNow.AddDays(2).Date, headResp.LifeCycleExpiresOn.Value.UtcDateTime.Date);
            Assert.Equal("AllExpire", headResp.LifeCycleRuleId);

            DeleteObjectResponse deleteResp = await ObjectClient.DeleteObjectAsync(BucketName, nameof(BasicCRDTest)).ConfigureAwait(false);
            Assert.Equal(204, deleteResp.StatusCode);
        }

        [Fact]
        public async Task GetWithContentRange()
        {
            await UploadAsync(nameof(GetWithContentRange)).ConfigureAwait(false);

            GetObjectResponse getResp = await ObjectClient.GetObjectAsync(BucketName, nameof(GetWithContentRange), req => req.Range.Add(0, 2)).ConfigureAwait(false);
            Assert.Equal("bytes", getResp.AcceptRanges);
            Assert.Equal("bytes 0-2/4", getResp.ContentRange);
            Assert.Equal("tes", await getResp.Content.AsStringAsync().ConfigureAwait(false));

            HeadObjectResponse headResp = await ObjectClient.HeadObjectAsync(BucketName, nameof(GetWithContentRange), req => req.Range.Add(0, 2)).ConfigureAwait(false);
            Assert.Equal("bytes", getResp.AcceptRanges);
            Assert.Equal("bytes 0-2/4", headResp.ContentRange);
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