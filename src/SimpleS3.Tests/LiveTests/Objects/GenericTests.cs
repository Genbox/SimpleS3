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
            PutObjectResponse pResp = await UploadAsync(nameof(BasicCRDTest)).ConfigureAwait(false);
            Assert.Equal(200, pResp.StatusCode);

            await AssertAsync(nameof(BasicCRDTest)).ConfigureAwait(false);

            DeleteObjectResponse dResp = await ObjectClient.DeleteObjectAsync(BucketName, nameof(BasicCRDTest)).ConfigureAwait(false);
            Assert.Equal(204, dResp.StatusCode);
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