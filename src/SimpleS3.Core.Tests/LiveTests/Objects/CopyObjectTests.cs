using System.Threading.Tasks;
using Genbox.SimpleS3.Core.Network.Responses.Objects;
using Xunit;
using Xunit.Abstractions;

namespace Genbox.SimpleS3.Core.Tests.LiveTests.Objects
{
    public class CopyObjectTests : LiveTestBase
    {
        public CopyObjectTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task CopyObject()
        {
            //Upload an object to copy
            string sourceKey = nameof(CopyObject);
            string destinationKey = sourceKey + "2";

            await UploadAsync(sourceKey).ConfigureAwait(false);

            CopyObjectResponse copyResp = await ObjectClient.CopyObjectAsync(BucketName, sourceKey, BucketName, destinationKey).ConfigureAwait(false);
            Assert.Equal(200, copyResp.StatusCode);

            await AssertAsync(destinationKey).ConfigureAwait(false);
        }
    }
}